 # Setup Firefox
echo 'pref("general.config.filename", "firefox.cfg");
pref("general.config.obscure_value", 0);' > ./autoconfig.js

echo '//Enable policies.json
lockPref("browser.policies.perUserDir", false);' > firefox.cfg

echo "{
    \"policies\": {
        \"Certificates\": {
            \"Install\": [
            	\"aspnetcore-localhost-https.crt\"
            ]
        }
    }
}" > policies.json

dotnet dev-certs https -ep localhost.crt --format PEM

sudo mv autoconfig.js /usr/lib64/firefox/
sudo mv firefox.cfg /usr/lib64/firefox/
sudo mv policies.json /usr/lib64/firefox/distribution/
mkdir -p ~/.mozilla/certificates
cp localhost.crt ~/.mozilla/certificates/aspnetcore-localhost-https.crt

# Trust Edge/Chrome
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n localhost -i ./localhost.crt
certutil -d sql:$HOME/.pki/nssdb -A -t "C,," -n localhost -i ./localhost.crt

# Trust dotnet-to-dotnet (.pem extension is important here)
sudo cp localhost.crt /var/lib/ca-certificates/openssl/aspnetcore-localhost-https.pem

# Cleanup
rm localhost.crt
