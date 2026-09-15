namespace CodeGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

            var template = new Templates.CustomerTemplate().ConfigureTemplate();

            if (!template.IsValid())
            {
                Console.WriteLine($"Invalid Template: {template.ToEntity}");
            }

            var backend = new BackendGenerator(template);
            backend.Generate();
        }
    }
}
