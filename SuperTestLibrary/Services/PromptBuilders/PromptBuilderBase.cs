﻿using SuperTestLibrary.Helpers;

namespace SuperTestLibrary.Services.PromptBuilders
{
    public abstract class PromptBuilderBase : IPromptBuilder
    {
        protected Prompt? _prompt;

        public IEnumerable<string> BuildPrompt(Prompt prompt)
        {
            ArgumentNullException.ThrowIfNull(prompt);
            _prompt = prompt;

            var prompts = new List<string> { BuildContext() };

            if (prompt.Interactions.Any())
            {
                prompts.AddRange(GetListOfInteractions.BuildInteractions(prompt));
            }

            return prompts;
        }

        protected abstract string BuildContext();
    }
}
