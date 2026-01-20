UNITY_PROJECT := /Users/developer/Projects3/mighty-ui
DLL_TARGET := $(UNITY_PROJECT)/Assets/Plugins/ExCSS.dll

.PHONY: build test sync

build:
	dotnet build src/ExCSS/ExCSS.csproj -c Release -f netstandard2.0

test:
	dotnet test --verbosity quiet

sync: build
	@mkdir -p $(UNITY_PROJECT)/Assets/Plugins
	cp src/ExCSS/bin/Release/netstandard2.0/ExCSS.dll $(DLL_TARGET)
	@echo "Synced to $(DLL_TARGET)"
