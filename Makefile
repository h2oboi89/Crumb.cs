SHELL := C:\Windows\System32\cmd.exe

PACKAGE_ROOT := $(shell echo $(USERPROFILE))\.nuget\packages

TDD_DIR := .\CodeCoverage
COVERAGE_REPORT_TOOL := $(PACKAGE_ROOT)\reportgenerator\5.1.24\tools\net6.0\ReportGenerator.exe
COVERAGE_REPORT := coverage.cobertura.xml

CONFIG := Debug

.PHONY: clean
clean:
	dotnet clean -c $(CONFIG) --nologo -v m

.PHONY: build
build: clean
	dotnet build -c $(CONFIG) --nologo -v m

.PHONY: tdd
tdd: build
	if exist $(TDD_DIR) ( rmdir /s /q $(TDD_DIR) )
	mkdir $(TDD_DIR)
	cd UnitTests && dotnet test --no-build --nologo  -v m /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:ExcludeByAttribute="GeneratedCodeAttribute"
	move ".\UnitTests\$(COVERAGE_REPORT)" "$(TDD_DIR)\$(COVERAGE_REPORT)"
	$(COVERAGE_REPORT_TOOL) -reports:$(TDD_DIR)\$(COVERAGE_REPORT) -targetdir:$(TDD_DIR) -verbosity:Warning -tag:$(GIT_LONG_HASH)

.PHONY: debug
debug : tdd

.PHONY: release
release : CONFIG := Release
release : tdd