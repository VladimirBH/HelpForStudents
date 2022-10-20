Сборка контейнера:
`docker build -f WebApi/Dockerfile -t webapi .`

Запуск контейнера (Windows):
`docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="43279HelpOurSTUdEnt42184" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/webapihttps.pfx -v D:\for_work\HelpForStudents\HttpsCertificate:/https/ webapi`

Запуск контейнера (Linux/MacOS):
`docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="43279HelpOurSTUdEnt42184" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/webapihttps.pfx -v /home/Documents/HelpForStudents/HttpsCertificate:/https/ webapi`

Запуск Docker-Compose:
`docker-compose build`
`docker-compose up`