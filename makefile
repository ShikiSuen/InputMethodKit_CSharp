.PHONY: all

all:
	dotnet clean && dotnet restore && dotnet build --no-restore && dotnet test --no-build

.PHONY: format pack

format:
	find . -regex '.*\.\(cs\)' -exec clang-format -style=file -i {} \;
pack:
	dotnet build --configuration Release --no-restore ; dotnet test ; dotnet pack --configuration Release
trim:
	find . -name "InputMethodKit.framework" -exec rm -rf {} \; || true
