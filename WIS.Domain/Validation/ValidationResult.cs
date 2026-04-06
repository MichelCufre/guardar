namespace WIS.Domain.Validation
{
    public class ValidationResult<T>
    {
        public T Value { get; set; }
        public string Error { get; set; }
    }
}
