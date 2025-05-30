#!/usr/bin/env bash
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Default values
BUILD_CONFIG="Debug"
CATEGORIES=()
WEB_URL=""
MAILPIT_URL=""
SMTP_SERVER=""
SMTP_PORT=""

# Help function
show_help() {
    echo "Usage: $0 [OPTIONS]"
    echo
    echo "Options:"
    echo "  -c, --config CONFIG     Build configuration (Debug/Release) [default: Debug]"
    echo "  -t, --tests TESTS       Comma-separated list of test categories (required)"
    echo "  -w, --web-url URL      Web server URL to use for tests"
    echo "  -m, --mailpit URL      Mailpit web interface URL"
    echo "  -s, --smtp-server HOST  SMTP server hostname"
    echo "  -p, --smtp-port PORT   SMTP server port"
    echo "  -h, --help             Show this help message"
    echo
    echo "Example:"
    echo "  $0 --config Debug --tests 'Smoke,UnitTests'"
}

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--config)
            BUILD_CONFIG="$2"
            shift 2
            ;;
        -t|--tests)
            IFS=',' read -ra CATEGORIES <<< "$2"
            shift 2
            ;;
        -w|--web-url)
            WEB_URL="$2"
            shift 2
            ;;
        -m|--mailpit)
            MAILPIT_URL="$2"
            shift 2
            ;;
        -s|--smtp-server)
            SMTP_SERVER="$2"
            shift 2
            ;;
        -p|--smtp-port)
            SMTP_PORT="$2"
            shift 2
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Validate build configuration
if [[ ! "$BUILD_CONFIG" =~ ^(Debug|Release)$ ]]; then
    echo "Error: Build configuration must be either 'Debug' or 'Release'"
    exit 1
fi

# Validate that test categories were provided
if [ ${#CATEGORIES[@]} -eq 0 ]; then
    echo "Error: No test categories specified"
    echo "Please provide test categories using the -t or --tests option"
    show_help
    exit 1
fi

# Set environment variables if parameters were provided
if [ -n "$WEB_URL" ]; then
    export WebServer__Url="$WEB_URL"
fi
if [ -n "$MAILPIT_URL" ]; then
    export MailPit__Url="$MAILPIT_URL"
fi
if [ -n "$SMTP_SERVER" ]; then
    export MailPit__SmtpServer="$SMTP_SERVER"
fi
if [ -n "$SMTP_PORT" ]; then
    export MailPit__SmtpPort="$SMTP_PORT"
fi

# Build the project first
echo "Building project with configuration: $BUILD_CONFIG"
dotnet build $SCRIPT_DIR/../../src --configuration "$BUILD_CONFIG"

# Run tests for each category
for category in "${CATEGORIES[@]}"; do
    echo "Running tests for category: $category"
    dotnet test $SCRIPT_DIR/../../src --no-build --configuration "$BUILD_CONFIG" --filter "Category=$category"
done