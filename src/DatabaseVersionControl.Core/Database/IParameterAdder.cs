using System;

namespace DatabaseVersionControl.Core.Database
{
    public interface IParameterAdder
    {
        void AddWithValue(string name, string value);
        void AddWithValue(string name, double value);
        void AddWithValue(string name, DateTime value);
    }
}