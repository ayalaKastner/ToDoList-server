# השתמש בתמונה בסיסית של .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# הגדר את תיקיית העבודה
WORKDIR /app

# העתק את קובץ הפרויקט והתקן את התלויות
COPY *.csproj ./
RUN dotnet restore

# העתק את שאר הקבצים ו-build את האפליקציה
COPY . ./
RUN dotnet publish -c Release -o out

# השתמש בתמונה בסיסית של .NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# הגדר את תיקיית העבודה
WORKDIR /app

# העתק את האפליקציה מהשלב הקודם
COPY --from=build /app/out .

# הגדר את הפורט שיישומך יאזין עליו
EXPOSE 5185

# הפעל את האפליקציה
ENTRYPOINT ["dotnet", "Program.dll"]
