using SuperTestLibrary.Services.Prompts;

namespace SuperTestLibrary.Helpers
{
    public static class GetListOfInteractions
    {
        public static IEnumerable<string> BuildInteractions(Prompt prompt)
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
