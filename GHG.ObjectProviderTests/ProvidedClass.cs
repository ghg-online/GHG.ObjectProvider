namespace GHG.ObjectProvider
{
    public interface IAdder
    {
        string ApplySuffix(string a);
        IAdder CreateSuffix(string a);
        IAdder Compose(IAdder adder1, IAdder adder2);
    }

    [GhgClass]
    public class Adder : IAdder
    {
        private readonly string? suffix;
        private readonly IAdder? adder1;
        private readonly IAdder? adder2;

        public Adder()
        {
            suffix = null;
            adder1 = null;
            adder2 = null;
        }

        Adder(string suffix)
        {
            this.suffix = suffix;
            adder1 = null;
            adder2 = null;
        }

        Adder(IAdder adder1, IAdder adder2)
        {
            suffix = null;
            this.adder1 = adder1;
            this.adder2 = adder2;
        }

        [GhgMethod]
        public string ApplySuffix(string a)
        {
            string value = a + suffix;
            if (adder1 != null)
                value = adder1.ApplySuffix(value);
            if (adder2 != null)
                value = adder2.ApplySuffix(value);
            return value;
        }

        [GhgMethod]
        public IAdder CreateSuffix(string a)
        {
            return new Adder(a);
        }

        [GhgMethod]
        public IAdder Compose(IAdder adder1, IAdder adder2)
        {
            return new Adder(adder1, adder2);
        }
    }
}
