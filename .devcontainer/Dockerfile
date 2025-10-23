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

# Install GitHub CLI
RUN (type -p wget >/dev/null || (apt update && apt install wget -y)) \
	&& mkdir -p -m 755 /etc/apt/keyrings \
	&& out=$(mktemp) && wget -nv -O$out https://cli.github.com/packages/githubcli-archive-keyring.gpg \
	&& cat $out | tee /etc/apt/keyrings/githubcli-archive-keyring.gpg > /dev/null \
	&& chmod go+r /etc/apt/keyrings/githubcli-archive-keyring.gpg \
	&& mkdir -p -m 755 /etc/apt/sources.list.d \
	&& echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
	&& apt update \
	&& apt install gh -y    