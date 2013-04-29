using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;

namespace DatabaseVersionControl.Cmd
{
    internal interface IOptionReader
    {
        string ReadLine(string key);
        void SaveValue(string key, string value);
        void ResetValue(string key);
        bool ContainsOption(string key);
    }

    public class OptionMemoryFile : IOptionReader , IDisposable
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const char Splitter = '|';
        private readonly string _exportSettingsFile;
        private bool _useFile;
        private IDictionary<string, string> _key;

        public OptionMemoryFile(string exportSettingsFile)
        {
            _exportSettingsFile = exportSettingsFile;
            _useFile = !string.IsNullOrEmpty(_exportSettingsFile);
            _key = new Dictionary<string, string>();
            if (_useFile){
                ReadFile();
            }
        }

        private void ReadFile()
        {
            if (File.Exists(_exportSettingsFile)){
                var readAllLines = File.ReadAllLines(_exportSettingsFile);
                foreach (var readAllLine in readAllLines){
                    var strings = readAllLine.Split(Splitter);
                    if (strings.Length == 2){
                        _key.Add(strings[0], strings[1]);
                    }
                }
            }
        }

        #region Implementation of IOptionReader

        public string ReadLine(string key)
        {
            if (_useFile && _key.ContainsKey(key))
            {
                return _key[key];
            }
            return Console.In.ReadLine();

        }

        public void SaveValue(string key, string value)
        {
            if (!_key.ContainsKey(key)){
                _key[key] = value;
                WriteFile();
            }
        }

        public void ResetValue(string key)
        {
            _key.Remove(key);
            WriteFile();
        }

        public bool ContainsOption(string key)
        {
            return _useFile && _key.ContainsKey(key);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (_useFile)
            {
                WriteFile();
            }
            
        }

        private void WriteFile()
        {
            if (_useFile){
                Log.Info("Write to settings file " + _exportSettingsFile);
                File.WriteAllLines(_exportSettingsFile, (_key.Select(x => x.Key + Splitter + x.Value)).ToArray());
            }
        }

        #endregion
    }
}