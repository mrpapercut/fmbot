# Custom Changes
This file describes custom changes made by Mischa to the FMBot code, for reference in updates

### Adding database folder
./database/Dockerfile
```bash
FROM postgres:14.1-alpine3.15
```
./database/data/.gitkeep
```bash
<empty>
```

### Adding database to docker-compose
./docker-compose.yml
```yml
version: '3.4'

services:
  fmbot:
    build:
      context: .
      dockerfile: src/Dockerfile
    container_name: fmbot_bot
    volumes:
      - "./config.json:/app/configs/config.json"
    depends_on:
      - postgres

  postgres:
    build:
      context: ./database
      dockerfile: Dockerfile
    container_name: fmbot_db
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      PGDATA: ${PGDATA}
    volumes:
      - "./database/data:/var/lib/postgresql/data"
    ports:
      - "5432:5432"
```

### Adding ./database/data, .gitkeep to gitignore
./.gitignore
```
database/data/

# Keep .gitkeep
!.gitkeep
```

### Add constant for DaysAlbumLastUsedForFeatured
./src/FMBot.Domain/Constants.cs L43
```cs
public const int DaysAlbumLastUsedForFeatured = 7;
```

### Lower threshold TotalListeners for featured albums
./src/FMBot.Bot/Services/FeaturedService.cs L249
```cs
if (!album.Success || album.Content == null || album.Content.TotalListeners < 250)
```

### Use constant DaysAlbumLastUsedForFeatured
./src/FMBot.Bot/Services/FeaturedService.cs L337
```cs
var filterDate = DateTime.UtcNow.AddDays(-Constants.DaysAlbumLastUsedForFeatured);
```

### Add if-statement for length of AlbumList
Prevents bug when switching featured image. 
./src/FMBot.Bot/Services/FeaturedService.cs L166
```cs
if (albumList.Count() > 0) {
```
