namespace SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects
{
    public class Name
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;

        public Name() { }

        public Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentException("FirstName cannot be empty");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new ArgumentException("LastName cannot be empty");
        }

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
