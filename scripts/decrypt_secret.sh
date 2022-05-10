#!/bin/sh

# Decrypt the file
mkdir $HOME/secrets
# --batch to prevent interactive command
# --yes to assume "yes" for questions
gpg --quiet --batch --yes --decrypt --passphrase="$CONFIG_SECRET" \
--output $HOME/secrets/config.json ../config/config.json.gpg
