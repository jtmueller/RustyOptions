# Just: https://github.com/casey/just

# Select a target to run
default:
  @just --choose

# Build the project
build *FLAGS:
  dotnet build {{FLAGS}} src/

# Generate a NuGet package
pack:
  dotnet pack -c Release src/

# Run tests in .NET 8.0
test:
  dotnet test -f net8.0 src/

# Clean build artifacts
clean:
  dotnet clean src/

# Build documentation
doc:
  dotnet build -c Release src/
  docfx metadata docs/docfx.json
  docfx build docs/docfx.json
