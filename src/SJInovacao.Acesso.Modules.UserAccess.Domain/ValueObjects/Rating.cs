namespace SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects
{
    public class Rating
    {
        public int Rate { get; set; } = 0;
        public string Count { get; set; } = string.Empty;
        public Rating() { }
        public Rating(int rate, string count)
        {
            Rate = rate;
            Count = count;
        }

    }
}
