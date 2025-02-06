#!/bin/bash
# Start the Ollama server in the background
ollama serve &

# Wait for the server to initialize
sleep 3

# Run the model interactively
exec ollama run deepseek-r1:1.5b