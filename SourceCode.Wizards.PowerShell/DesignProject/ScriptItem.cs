using System;
using System.Text;
using SourceCode.Framework;
using SourceCode.Workflow.Authoring;

namespace SourceCode.Wizard.PowerShell.Design
{
    public enum ScriptType
    {
        File,
        Script
    }

    public class ScriptItem : PersistableContainerObject
    {
        private const string SCRIPT = "ScriptContent";
        private const string TYPE = "ScriptType";
        private const string LOCATION = "ScriptLocation";
        private K2Field _scriptContent;
        private ScriptType _type;
        private string _fileLocation;

        #region Constructors/Dispose
        public ScriptItem()
            : base()
        {
            Type = ScriptType.Script;
        }

        public ScriptItem(K2Field script)
            : this()
        {
            Type = ScriptType.Script;
            Script = script;
        }
        public ScriptItem(string fileLocation)
        {
            Type = ScriptType.File;
            FileLocation = fileLocation;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_scriptContent != null)
                {
                    _scriptContent.Dispose();
                }
            }
            _scriptContent = null;

            base.Dispose(disposing);
        }

        #endregion Constructors/Dispose

        #region Public properties
        public ScriptType Type
        {
            get
            {
                return _type;
            }
            set
            {
                base.OnNotifyPropertyChanged(TYPE, ref _type, value);
            }
        }

        public K2Field Script
        {
            get
            {
                if (_scriptContent == null)
                {
                    _scriptContent = new K2Field(this);
                }
                return _scriptContent;
            }
            set
            {
                base.OnNotifyPropertyChanged(SCRIPT, ref _scriptContent, value);
            }
        }

        public string FileLocation
        {
            get
            {
                if (string.IsNullOrEmpty(_fileLocation))
                {
                    //TODO: Throw exception?
                    _fileLocation = string.Empty;
                }
                return _fileLocation;
            }
            set
            {
                base.OnNotifyPropertyChanged(LOCATION, ref _fileLocation, value);
            }
        }

        #endregion Public properties


        #region Save/Load/etc overrides
        protected override void OnSave(ISerializationInfo content)
        {
            base.OnSave(content);
            content.SetProperty(TYPE, _type);
            if (!string.IsNullOrEmpty(_fileLocation))
            {
                content.SetProperty(LOCATION, _fileLocation);
            }
            if (!K2Field.IsNullOrEmpty(_scriptContent))
            {
                content.SetProperty(SCRIPT, _scriptContent);
            }
        }


        protected override void OnLoad(ISerializationInfo content)
        {
            base.OnLoad(content);

            if (content.HasProperty(TYPE))
            {
                Type = (ScriptType)content.GetPropertyAsEnum(TYPE, typeof(ScriptType));
            }
            if (content.HasProperty(LOCATION))
            {
                FileLocation = content.GetPropertyAsString(LOCATION);
            }

            if (content.HasProperty(SCRIPT))
            {
                Script = (K2Field)content.GetPropertyAsPersistableObject(SCRIPT);
            }
        }

        public override T Clone<T>()
        {
            ScriptItem item = base.Clone<ScriptItem>();
            if (Script != null)
            {
                item.Script = Script.Clone<K2Field>();
            }
            if (!string.IsNullOrEmpty(FileLocation))
            {
                item.FileLocation = FileLocation;
            }
            item.Type = this.Type;

            return item as T;
        }



        protected override System.Collections.IEnumerable ContainedList
        {
            get { throw new NotImplementedException(); }
        }
        #endregion Save/Load/etc overrides
    }
}
