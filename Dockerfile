# Use the ASP.NET 9.0 runtime-only image (very lightweight)
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Set working directory
WORKDIR /app

# Copy only the published output
COPY ./publish .

# Expose the port Cloud Run expects
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "ctf-sandbox.dll"]