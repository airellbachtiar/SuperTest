namespace SuperTestLibrary.Services.Prompts.Builders
{
    public class PromptBuilderBase
    {
        internal IEnumerable<string> BuildInteractions(Prompt prompt)
        {
            List<string> interactions = [];

            foreach (var interaction in prompt.Interactions)
            {
                interactions.Add(interaction.Message);
            }

            return interactions;
        }
    }
}
