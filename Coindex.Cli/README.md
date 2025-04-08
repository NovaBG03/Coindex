# Coindex CLI

A simple command-line interface for querying the Coindex collectables database. This CLI provides an easy way to access the same database used by the Coindex app, demonstrating code sharing between the app and CLI.

## Usage

Run the CLI with one of the following commands:

```bash
# Create alias
alias coindex="dotnet run --project Coindex.Cli"

# Create with custom db path
alias coindex="dotnet run --project Coindex.Cli --db /Users/nova/Library/Containers/com.companyname.coindex.app/Data/Documents/coindex.db3"

# Show help
coindex help

# List all collectables
coindex list

# List collectables with pagination
coindex list --page 2

# List collectables with tag filtering
coindex list --tag Rare

# List collectables with condition filtering
coindex list --condition Fine

# Show a specific collectable by ID
coindex show 1

# List all available tags
coindex tags

# Search for collectables by name
coindex search Test

# Show statistics about the collection
coindex stats
```
