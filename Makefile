UNITY_PROJECT := /Users/developer/Projects3/mighty-ui
DLL_TARGET := $(UNITY_PROJECT)/Assets/Plugins/ExCSS.dll

.PHONY: help build test clean sync

help:
	@echo "Available targets:"
	@echo "  make build  - Clean and build ExCSS.dll (Release, netstandard2.0)"
	@echo "  make test   - Run unit tests"
	@echo "  make clean  - Remove build artifacts"
	@echo "  make sync   - Clean, build and copy DLL to Unity project"

build: clean
	dotnet build src/ExCSS/ExCSS.csproj -c Release -f netstandard2.0

test:
	dotnet test --verbosity quiet

clean:
	dotnet clean src/ExCSS/ExCSS.csproj -c Release -v quiet
	@echo "Cleaned build artifacts"

sync: build
	@mkdir -p $(UNITY_PROJECT)/Assets/Plugins
	cp src/ExCSS/bin/Release/netstandard2.0/ExCSS.dll $(DLL_TARGET)
	@echo "Synced to $(DLL_TARGET)"
