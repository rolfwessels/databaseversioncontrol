namespace Intercontinental.Core.Database.Do
{
    public interface IChangedObject
    {
        bool IsChanged { get; }
        void SetChanged(bool inChanged);
    }
}