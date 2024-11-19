## Public password
It can be used to encrypt secret password in form of qr-code. Can be usefull if you want to save your password on paper but it's inconvenient to write encrypted text or unsafe to write pure secret password.

## Usage
Encryption:  
`./PublicPassword.Implementation.exe encrypt --password 1 --text helloworld --out-file x1.png`  
The "--text" param is encrypted by "--password" and encrypted result is saved in "--out-file".

Decryption:  
`./PublicPassword.Implementation.exe decrypt --password 1 --input-file x1.png`

## Publishing
`dotnet publish --configuration release --runtime win-x64 -p:PublishSingleFile=true --self-contained true`
