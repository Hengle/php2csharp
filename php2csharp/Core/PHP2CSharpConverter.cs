﻿using PHP2CSharp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PHP2CSharp.Core
{
    public class PHP2CSharpConverter
    {
        private IList<BaseConverter> _converters;

        public PHP2CSharpConverter() {
            _converters = new List<BaseConverter>() {
                new DBConverter(),
                new NamespaceConverter(),
                new ConstantConverter(),
                new ArrayConverter(),
                new PropertyConverter(),
                new VariableConverter(),
                new StringConverter(),
                new FunctionConverter(),
                new MethodConverter()
            };
        }

        private string clearCode(string sourceCode) {
            sourceCode = sourceCode.Replace("\r", "");
            var source = new StringBuilder();
            var lines = sourceCode.Trim().Split(new[] { '\n' });
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim())) {
                    source.Append("\n");
                }
                else
                {
                    source.AppendLine(line);
                }
            }
            sourceCode = source.ToString();
            sourceCode = sourceCode.Replace("\r", "");
            while (sourceCode.IndexOf("\n\n\n") >= 0) {
                sourceCode = sourceCode.Replace("\n\n\n", "\n");
            }
            sourceCode = sourceCode.Replace("\n", "\r\n");
            return sourceCode;
        }

        public string convert(string originalSource) {
            string source = originalSource;
            source = Regex.Replace(source, @"\<\?php", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, @"\?\>", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, @"class ([0-9,a-z,A-Z,_]+)", delegate (Match match) {
                return "public class " + match.Groups[1].Value;
            }, RegexOptions.IgnoreCase);
            foreach (var converter in _converters) {
                source = converter.convert(source);
            }
            source = Regex.Replace(source, @"/\*\*.*?\*/", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            source = Regex.Replace(source, @"\$([0-9,a-z,A-Z,_]+)", delegate (Match match) {
                return match.Groups[1].Value;
            }, RegexOptions.IgnoreCase);
            source = source.Replace('\'', '"');
            return clearCode(source);
        }
    }
}
