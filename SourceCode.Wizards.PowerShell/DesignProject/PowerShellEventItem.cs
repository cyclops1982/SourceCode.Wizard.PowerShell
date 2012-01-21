//////////////////////////////////////////////////////////////////////////
//																		//
//		This class is your main "interface" from the wizard.			//
//		All the "intelligence" of you wizard occurs here in terms		//
//		of what values/properties are saved when your project is 		//
//		saved and what values are sent through to the Extender			//
//																		//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

using SourceCode.Workflow.Authoring;
using SourceCode.Workflow.Design;
using SourceCode.Framework;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Microsoft.CSharp;
using SourceCode.Framework.Data;

namespace SourceCode.Wizard.PowerShell.Design
{
    public class PowerShellEventItem : EventItem
    {
        private const string POWERSHELLSCRIPT = "PowerShellScript";
        private const string INPUTVARIABLES = "InputVariables";
        private const string OUTPUTVARIABLES = "OutputVariables";
        private K2Field _powerShellScript;
        private VariableItemCollection _outputVariables;
        private VariableItemCollection _inputVariables;


        public PowerShellEventItem()
            : base()
        {
            base.Extender = new CodeExtender();
        }

        public K2Field PowerShellScript
        {
            get
            {
                if (_powerShellScript == null)
                {
                    _powerShellScript = new K2Field(this);
                }
                return _powerShellScript;
            }
            set
            {
                base.OnNotifyPropertyChanged(POWERSHELLSCRIPT, ref _powerShellScript, value);
            }
        }


        public VariableItemCollection InputVariables
        {
            get
            {
                if (_inputVariables == null)
                {
                    _inputVariables = new VariableItemCollection(this);
                }
                return _inputVariables;
            }
            set
            {
                base.OnNotifyPropertyChanged(INPUTVARIABLES, ref _inputVariables, value);
            }
        }

        public VariableItemCollection OutputVariables
        {
            get
            {
                if (_outputVariables == null)
                {
                    _outputVariables = new VariableItemCollection(this);
                }
                return _outputVariables;
            }
            set
            {
                base.OnNotifyPropertyChanged(OUTPUTVARIABLES, ref _outputVariables, value);
            }
        }


        #region Override Methods

        //TODO: Remove?
        protected override void Dispose(bool disposing)
        {
            if (base.IsDisposed)
            {
                return;
            }

            //Set pointer = null
            //_myK2Field1 = null;

            base.Dispose(disposing);
        }


        protected override void OnLoad(ISerializationInfo content)
        {
            base.OnLoad(content);

            if (content.HasProperty(POWERSHELLSCRIPT))
            {
                PowerShellScript = (K2Field)content.GetPropertyAsPersistableObject(POWERSHELLSCRIPT);
            }

            if (content.HasProperty(INPUTVARIABLES))
            {
                InputVariables.Clear();
                content.GetListProperty(INPUTVARIABLES, InputVariables);
            }

            if (content.HasProperty(OUTPUTVARIABLES))
            {
                OutputVariables.Clear();
                content.GetListProperty(OUTPUTVARIABLES, OutputVariables);
            }

        }

        protected override void OnSave(ISerializationInfo content)
        {
            base.OnSave(content);

            if (!K2Field.IsNullOrEmpty(_powerShellScript))
            {
                content.SetProperty(POWERSHELLSCRIPT, _powerShellScript);
            }

            if (_inputVariables != null && _inputVariables.Count > 0)
            {
                content.SetListProperty(INPUTVARIABLES, _inputVariables);
            }

            if (_outputVariables != null && _outputVariables.Count > 0)
            {
                content.SetListProperty(OUTPUTVARIABLES, _outputVariables);
            }
        }



        protected override void PrepareConfigurationForBuild()
        {

            base.PrepareConfigurationForBuild();

            base.Extender.Configuration[POWERSHELLSCRIPT] = Utility.CreateConfigSettingField<string>(_powerShellScript);
            base.Extender.Configuration[INPUTVARIABLES] = SerializeVariableList(InputVariables, INPUTVARIABLES);
            base.Extender.Configuration[OUTPUTVARIABLES] = SerializeVariableList(OutputVariables, OUTPUTVARIABLES);

            GenerateCodeFiles();


        }

        private void GenerateCodeFiles()
        {
            foreach (CodeFile file in base.Extender.CodeFiles)
            {
                if (file.OriginalFileName != null && file.OriginalFileName.EndsWith(".mapping.cs", StringComparison.CurrentCultureIgnoreCase))
                {
                    CodeCompileUnit code = this.GenerateCode(file);
                    CSharpCodeProvider provider = new CSharpCodeProvider();
                    using (StringWriter stream = new StringWriter())
                    {
                        provider.GenerateCodeFromCompileUnit(code, stream, new CodeGeneratorOptions());
                        file.Content = stream.ToString();
                    }
                }
            }
        }

        private CodeCompileUnit GenerateCode(CodeFile file)
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace ns = new CodeNamespace("$rootnamespace$");
            ns.Types.Add(GenerateClass(this));


            compileUnit.Namespaces.Add(ns);

            return compileUnit;
        }

        private CodeTypeDeclaration GenerateClass(PowerShellEventItem powerShellEventItem)
        {
            AssemblyName name = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            CodeTypeDeclaration type = new CodeTypeDeclaration("$safeitemname$")
            {
                IsPartial = true
            };

            type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)), new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression("K2")), new CodeAttributeArgument(new CodePrimitiveExpression(name.Version.ToString())) }));

            int outputMaps = 0;
            foreach (VariableItem item in powerShellEventItem.OutputVariables)
            {
                CodeTypeMember mem = GenerateOutputMapMethod(item, powerShellEventItem, outputMaps++);
                type.Members.Add(mem);
            }

            int inputMaps = 0;
            foreach (VariableItem item in powerShellEventItem.InputVariables)
            {
                CodeTypeMember mem = GenerateInputMapMethod(item, powerShellEventItem, inputMaps++);
                type.Members.Add(mem);
            }

            type.Members.Add(GeneratePerformMappingMethod(outputMaps, "OutputMap"));
            type.Members.Add(GeneratePerformMappingMethod(inputMaps, "InputMap"));

            return type;
        }

        private static CodeMemberMethod GeneratePerformMappingMethod(int mapCount, string methodName)
        {
            CodeParameterDeclarationExpression rsParam = new CodeParameterDeclarationExpression(new CodeTypeReference("System.Management.Automation.Runspaces.Runspace"), "rs");
            CodeParameterDeclarationExpression k2Param = new CodeParameterDeclarationExpression(new CodeTypeReference("$contexttype$"), "K2");

            CodeMemberMethod performMappingMethod = new CodeMemberMethod();
            performMappingMethod.Name = methodName;
            performMappingMethod.ReturnType = new CodeTypeReference(typeof(void));
            performMappingMethod.Parameters.Add(k2Param);
            performMappingMethod.Parameters.Add(rsParam);


            for (int i = 0; i < mapCount; i++)
            {
                CodeMethodInvokeExpression mapMethod = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), methodName + i.ToString());
                mapMethod.Parameters.Add(new CodeVariableReferenceExpression("K2"));
                mapMethod.Parameters.Add(new CodeVariableReferenceExpression("rs"));

                performMappingMethod.Statements.Add(mapMethod);
            }

            return performMappingMethod;
        }


        private CodeTypeMember GenerateInputMapMethod(VariableItem item, PowerShellEventItem powerShellEventItem, int mapNr)
        {
            CodeMemberMethod method = new CodeMemberMethod();

            CodeParameterDeclarationExpression rsParam = new CodeParameterDeclarationExpression(new CodeTypeReference("System.Management.Automation.Runspaces.Runspace"), "rs");
            CodeParameterDeclarationExpression k2Param = new CodeParameterDeclarationExpression(new CodeTypeReference("$contexttype$"), "K2");

            method.Name = "InputMap" + mapNr.ToString();
            method.ReturnType = new CodeTypeReference(typeof(void));
            method.Parameters.Add(k2Param);
            method.Parameters.Add(rsParam);

        

            // Some variables we need
            CodeVariableReferenceExpression k2Field = new CodeVariableReferenceExpression("K2");
            

            //Declare a object variable
            CodeVariableDeclarationStatement varValueDecl = new CodeVariableDeclarationStatement(typeof(object), "variableValue");
            CodeVariableReferenceExpression varValueRef = new CodeVariableReferenceExpression("variableValue");
            method.Statements.Add(varValueDecl);


            
            //TODO: use getsimplfy function? Check why that is there?
            K2FieldPart part = Simplify(item.VariableValue);
            string data = ((IRuntimeDataProvider)part).GetGetRuntimeData();

            // This creates a 'getvalue' method
            CodeFieldReferenceExpression resolverManagerProperty = new CodeFieldReferenceExpression(k2Field, "ResolverManager");
            CodeMethodInvokeExpression getValue = new CodeMethodInvokeExpression(
                resolverManagerProperty,
                "GetValue",
                new CodeExpression[] { new CodePrimitiveExpression(data) }
            );

            // Assign the getValue to a variable.
            CodeAssignStatement assignCurrentValue = new CodeAssignStatement();
            assignCurrentValue.Right = getValue;
            assignCurrentValue.Left = varValueRef;
            method.Statements.Add(assignCurrentValue);

            method.Statements.Add(new CodeSnippetStatement("  if (variableValue != null) {"));
            method.Statements.Add(new CodeSnippetStatement(string.Format("rs.SessionStateProxy.SetVariable(\"{0}\", variableValue);", item.Name)));
            method.Statements.Add(new CodeSnippetStatement("  }"));
       

            return method;
        }



        private CodeTypeMember GenerateOutputMapMethod(VariableItem item, PowerShellEventItem powerShellEventItem, int mapNr)
        {
            CodeMemberMethod method = new CodeMemberMethod();

            CodeParameterDeclarationExpression rsParam = new CodeParameterDeclarationExpression(new CodeTypeReference("System.Management.Automation.Runspaces.Runspace"), "rs");
            CodeParameterDeclarationExpression k2Param = new CodeParameterDeclarationExpression(new CodeTypeReference("$contexttype$"), "K2");

            method.Name = "OutputMap" + mapNr.ToString();
            method.ReturnType = new CodeTypeReference(typeof(void));
            method.Parameters.Add(k2Param);
            method.Parameters.Add(rsParam);



            method.Statements.Add(new CodeSnippetStatement(string.Format("object psVariable = rs.SessionStateProxy.GetVariable(\"{0}\");", item.Name)));
            method.Statements.Add(new CodeSnippetStatement("if (psVariable != null) {"));

            //TODO: use getsimplfy function? Check why that is there?
            K2FieldPart part = item.VariableValue.Parts[0];
            string data = ((IRuntimeDataProvider)part).GetGetRuntimeData();


            // Some variables we need
            CodeVariableReferenceExpression k2Field = new CodeVariableReferenceExpression("K2");
            CodeVariableReferenceExpression psVariable = new CodeVariableReferenceExpression("psVariable");


            // new object variable
            CodeVariableDeclarationStatement destVariableDeclaration = new CodeVariableDeclarationStatement(typeof(object), "destinationValue");
            method.Statements.Add(destVariableDeclaration);

            CodeVariableReferenceExpression destVariable = new CodeVariableReferenceExpression("destinationValue");


            // This creates a 'getvalue' method
            CodeFieldReferenceExpression resolverManagerProperty = new CodeFieldReferenceExpression(k2Field, "ResolverManager");
            CodeMethodInvokeExpression getValue = new CodeMethodInvokeExpression(
                resolverManagerProperty,
                "GetValue",
                new CodeExpression[] { new CodePrimitiveExpression(data) }
            );


            // Assign the getValue to a variable.
            CodeAssignStatement assignCurrentValue = new CodeAssignStatement();
            assignCurrentValue.Right = getValue;
            assignCurrentValue.Left = destVariable;
            method.Statements.Add(assignCurrentValue);

            //TODO: convert to statements
            method.Statements.Add(new CodeSnippetStatement("  if (psVariable.GetType() != destinationValue.GetType()) {"));
            method.Statements.Add(new CodeSnippetStatement("    K2.ProcessInstance.Logger.LogDebugMessage(\"PowerShellWizard\", \"Source and destination type of PowerShell Variable " + item.Name + " do not match. Doing conversion.\");"));

 
            // Do the system.Convert.ChangeType method.         
            CodeTypeReferenceExpression systemConvert = new CodeTypeReferenceExpression("System.Convert");
            CodeMethodInvokeExpression changeTypeInvoke = new CodeMethodInvokeExpression(
                systemConvert,
                "ChangeType",
                new CodeExpression[] {
                    psVariable,
                    new CodeMethodInvokeExpression(destVariable, "GetType")
                });

            CodeAssignStatement convertAssign = new CodeAssignStatement();
            convertAssign.Left = psVariable;
            convertAssign.Right = changeTypeInvoke;
            method.Statements.Add(convertAssign);


            //TODO: convert to statements
            method.Statements.Add(new CodeSnippetStatement("    if (psVariable == null) {"));
            method.Statements.Add(new CodeSnippetStatement("      throw new System.Exception(\"Conversion of variable '" + item.Name + "' failed. Please make sure the types are the same.\");"));
            method.Statements.Add(new CodeSnippetStatement("    }"));



            method.Statements.Add(new CodeSnippetStatement("  }"));



            // Create a setValue on the ResolverManager
            CodeMethodInvokeExpression setValue = new CodeMethodInvokeExpression(
                resolverManagerProperty,
                "SetValue",
                new CodeExpression[] { 
                    new CodePrimitiveExpression(data), 
                    psVariable
                });
            method.Statements.Add(setValue);

            // Commit the resolverManager
            CodeMethodInvokeExpression commit = new CodeMethodInvokeExpression(
                resolverManagerProperty,
                "Commit"
            );
            method.Statements.Add(commit);

            method.Statements.Add(new CodeSnippetStatement("}"));

            return method;
        }

        private Type ResolveTypeSupportingObject(K2Field field)
        {
            Type valueType;
            if (Simplify(field) == null)
            {
                valueType = typeof(string);
            }
            else
            {
                valueType = Simplify(field).ValueType;
            }
            if (valueType == null)
            {
                valueType = typeof(object);
            }
            if (valueType.IsArray && (valueType != typeof(byte[])))
            {
                valueType = valueType.GetElementType();
            }
            if (valueType == typeof(Uri))
            {
                valueType = typeof(string);
            }
            return valueType;
        }






        private K2FieldPart Simplify(K2FieldPart fieldPart)
        {
            if (!(fieldPart is K2Field))
            {
                return fieldPart;
            }

            K2Field field = (K2Field)fieldPart;
            while (field.Parts.Count > 0)
            {
                if (field.Parts.Count != 1)
                {
                    return fieldPart;
                }
                if (field.Parts[0] is K2Field)
                {
                    field = (K2Field)field.Parts[0];
                }
                else
                {
                    return field.Parts[0];
                }
            }

            return field;

        }



        private K2Field SerializeVariableList(VariableItemCollection list, string name)
        {
            XmlPartWriter writer = new XmlPartWriter();

            writer.WriteStartElement(name);

            if (_inputVariables != null)
            {
                foreach (VariableItem item in list)
                {
                    writer.WriteStartElement("VariableItem");
                    writer.WriteAttributeValue("Name", item.Name);
                    writer.WriteAttributeValue("Value", item.VariableValue);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();

            return new K2Field(writer.Parts);
        }

        public override T Clone<T>()
        {
            PowerShellEventItem item = base.Clone<PowerShellEventItem>();
            if (!K2Field.IsNullOrEmpty(_powerShellScript))
            {
                item.PowerShellScript = _powerShellScript.Clone<K2Field>();
            }

            if (_inputVariables != null)
            {
                foreach (VariableItem varItem in _inputVariables)
                {
                    item.InputVariables.Add(varItem.Clone<VariableItem>());
                }
            }

            if (_outputVariables != null)
            {
                foreach (VariableItem varItem in _outputVariables)
                {
                    item.OutputVariables.Add(varItem.Clone<VariableItem>());
                }
            }

            return item as T;

        }



        protected override void PrepareCodeFiles()
        {
            base.PrepareCodeFiles();

            if (base.Extender != null && base.Extender.Process != null)
            {
                if (base.Extender.CodeFiles.Count == 0)
                {
                    using (CodeFileResolver resolver = new CodeFileResolver(base.Extender, @"SourceCode.Wizard.PowerShell.SourceCode.Wizard.PowerShellEventItem\EventItem"))
                    {
                        string genFile = string.Empty;
                        foreach (CodeFile codeFile in resolver.GetCodeFilesFromDisk())
                        {
                            if (Path.GetExtension(codeFile.FileName) == ".cs")
                            {
                                genFile = Path.ChangeExtension(codeFile.OriginalFileName, ".mapping.cs");
                            }
                            base.Extender.CodeFiles.Add(codeFile);
                        }

                        if (string.IsNullOrEmpty(genFile))
                        {
                            throw new ApplicationException("We tried to generate a .mapping.cs file, but we couldn't really do that. Sorry.");
                        }
                        CodeFile newFile = new CodeFile();
                        newFile.OriginalFileName = genFile;
                        base.Extender.CodeFiles.Add(newFile);
                    }
                }
                else
                {
                    this.GenerateCodeFiles();
                }
            }
        }

        protected override string[] References
        {
            get
            {
                List<string> refs = new List<string>();
                refs.Add("System.Management.Automation");
                return refs.ToArray();
            }
        }

        #endregion
    }
}
