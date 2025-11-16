# LemmikkiAPI
dotnet new ignore 
dotnet new webapi -n LemmikkiAPI

Ohjeet Dockerin käynnistykseen:

Ensin buildaa image:
Käynnistä Docker
Mene kansioon jossa Dockerfile sijaitsee. 
Aja komento
docker build -t lemmikkikanta-api .

Käynnistä kontti:
docker run -d -p 8080:5215 --name lemmikki-kontti lemmikkikanta-api