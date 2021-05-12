using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hopper.Meta.Template 
{
    public class CodePrinterBase 
    {
        private StringBuilder builder;
        private IDictionary<string, object> session;
        private CompilerErrorCollection errors;
        private string currentIndent = string.Empty;
        private Stack<int> indents;
        private ToStringInstanceHelper _toStringHelper = new ToStringInstanceHelper();
        
        public virtual IDictionary<string, object> Session 
        {
            get { return this.session; }
            set { this.session = value; }
        }
        
        public StringBuilder GenerationEnvironment 
        {
            get 
            {
                if (this.builder is null) 
                {
                    this.builder = new StringBuilder();
                }
                return this.builder;
            }
            set {
                this.builder = value;
            }
        }
        
        protected CompilerErrorCollection Errors 
        {
            get 
            {
                if (this.errors is null) 
                {
                    this.errors = new CompilerErrorCollection();
                }
                return this.errors;
            }
        }
        
        public string CurrentIndent 
        {
            get 
            {
                return this.currentIndent;
            }
        }
        
        private Stack<int> Indents 
        {
            get 
            {
                if (this.indents is null) 
                {
                    this.indents = new Stack<int>();
                }
                return this.indents;
            }
        }
        
        public ToStringInstanceHelper ToStringHelper => this._toStringHelper;
        
        public void Error(string message) 
        {
            Errors.Add(new CompilerError(null, -1, -1, null, message));
        }
        
        public void Warning(string message) 
        {
            CompilerError val = new CompilerError(null, -1, -1, null, message);
            val.IsWarning = true;
            Errors.Add(val);
        }
        
        public string PopIndent() 
        {
            if (Indents.Count == 0) 
            {
                return string.Empty;
            }
            int lastPos = currentIndent.Length - Indents.Pop();
            string last = currentIndent.Substring(lastPos);
            currentIndent = currentIndent.Substring(0, lastPos);
            return last;
        }
        
        public void PushIndent(string indent = "  ") 
        {
            Indents.Push(indent.Length);
            currentIndent = currentIndent + indent;
        }
        
        public void ClearIndent() 
        {
            currentIndent = string.Empty;
            Indents.Clear();
        }
        
        public void Write(string textToAppend) 
        {
            GenerationEnvironment.Append(textToAppend);
        }
        
        public void Write(string format, params object[] args) 
        {
            GenerationEnvironment.AppendFormat(format, args);
        }
        
        public void WriteLine(string textToAppend) 
        {
            GenerationEnvironment.Append(currentIndent);
            GenerationEnvironment.AppendLine(textToAppend);
        }
        
        public void WriteLine(string format, params object[] args) 
        {
            GenerationEnvironment.Append(currentIndent);
            GenerationEnvironment.AppendFormat(format, args);
            GenerationEnvironment.AppendLine();
        }

        public void WriteLines(IEnumerable<string> strings)
        {
            foreach (var s in strings)
            {
                GenerationEnvironment.Append(currentIndent);
                GenerationEnvironment.AppendLine(s);
            }
        }

        public void WriteLinesCommaSeparated(IEnumerable<string> strings)
        {
            PushIndent();
            GenerationEnvironment.Append(String.Join(",\n{currentIndent}", strings));
            PopIndent();
        }

        public virtual string TransformText()
        {
            return string.Empty;
        }

        // TODO: do stream writes
        public void WriteToFile(string fileName)
        {
            File.WriteAllText(fileName, TransformText(), Encoding.UTF8);
        }

        public virtual void Initialize() {}
        
        public class ToStringInstanceHelper 
        {
            private IFormatProvider formatProvider = System.Globalization.CultureInfo.InvariantCulture;
            
            public IFormatProvider FormatProvider => formatProvider;
                // set {
                //     if ((value != null)) {
                //         this.formatProvider = value;
                //     }
                // }
            
            public string ToStringWithCulture(object objectToConvert) 
            {
                if (objectToConvert is null) 
                {
                    throw new ArgumentNullException("objectToConvert");
                }
                Type type = objectToConvert.GetType();
                Type iConvertibleType = typeof(IConvertible);
                
                if (iConvertibleType.IsAssignableFrom(type)) 
                {
                    return ((IConvertible)(objectToConvert)).ToString(formatProvider);
                }

                var methInfo = type.GetMethod("ToString", new Type[] {iConvertibleType});
                
                if (!(methInfo is null)) 
                {
                    return ((string)(methInfo.Invoke(objectToConvert, new object[] {
                                this.formatProvider})));
                }

                return objectToConvert.ToString();
            }
        }
    }
}