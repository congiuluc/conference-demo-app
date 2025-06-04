#!/bin/bash

# Clean previous test results
rm -rf TestResults/*

echo "Running tests for all projects..."

# Run tests for each project
echo "Testing ConferenceApp.Shared..."
dotnet test ConferenceApp.Shared.Tests/ConferenceApp.Shared.Tests.csproj --collect:"XPlat Code Coverage" --results-directory TestResults

echo "Testing ConferenceApp.API..."
dotnet test ConferenceApp.API.Tests/ConferenceApp.API.Tests.csproj --collect:"XPlat Code Coverage" --results-directory TestResults

echo "Testing ConferenceApp.ServiceDefaults..."
dotnet test ConferenceApp.ServiceDefaults.Tests/ConferenceApp.ServiceDefaults.Tests.csproj --collect:"XPlat Code Coverage" --results-directory TestResults

echo "Generating coverage report..."
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:HtmlInline_AzurePipelines_Dark,TextSummary

echo "Coverage report generated in CoverageReport directory"
echo "Coverage Summary:"
cat CoverageReport/Summary.txt