FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# Install playwright dependencies (MAKE SURE TO KEEP PLAYWRIGHT VERSIONS IN SYNC WITH .NET PROJECTS)
RUN mkdir /tmp/installplaywright && \
    cd /tmp/installplaywright && \
    dotnet new xunit && \
    dotnet add package Microsoft.Playwright -v 1.54.0 && \
    dotnet build && \
    pwsh ./bin/Debug/net9.0/playwright.ps1 install-deps

# Install Playwright browsers
USER vscode
RUN pwsh /tmp/installplaywright/bin/Debug/net9.0/playwright.ps1 install

USER root
# Install Google Cloud CLI and kubectl
RUN apt-get update && apt-get install -y apt-transport-https ca-certificates gnupg curl lsb-release && \
    echo "deb [signed-by=/usr/share/keyrings/cloud.google.gpg] https://packages.cloud.google.com/apt cloud-sdk main" \
    | tee /etc/apt/sources.list.d/google-cloud-sdk.list && \
    curl https://packages.cloud.google.com/apt/doc/apt-key.gpg \
    | apt-key --keyring /usr/share/keyrings/cloud.google.gpg add - && \
    apt-get update && apt-get install -y google-cloud-cli && \
    apt-get install google-cloud-cli-gke-gcloud-auth-plugin && \
    apt-get install kubectl