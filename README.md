# TaskTracker

Система управления проектами и задачами с разграничением доступа по ролям.

## Стек

- **Backend:** ASP.NET Core 10, Entity Framework Core, MS SQL Server, JWT
- **Desktop/Mobile-клиент:** .NET MAUI 10 (Windows, Android, iOS, macOS)
- **Инфраструктура:** Docker, Docker Compose, Caddy (HTTPS reverse-proxy)

---

## Быстрый старт (backend)

### Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

### 1. Клонируйте репозиторий

```bash
git clone https://github.com/yourname/tasktracker.git
cd tasktracker
```

### 2. Создайте файл `.env`

```bash
cp .env.example .env
```

Откройте `.env` и заполните значения:

```env
DB_CONNECTION_STRING=Server=db,1433;Database=TaskTrackerDb;User Id=sa;Password=YourStr0ng!DbPassword;TrustServerCertificate=True
DB_PASSWORD=YourStr0ng!DbPassword
JWT_KEY=YourSuperSecretJwtKey_AtLeast32Characters_Long!
ADMIN_EMAIL=admin@yourdomain.com
ADMIN_PASSWORD=YourStr0ng!AdminPassword
DOMAIN=
ACME_EMAIL=
API_PORT=8080
```

> ⚠️ `DB_PASSWORD` должен содержать заглавные/строчные буквы, цифры и спецсимволы — требование MS SQL Server.
> ⚠️ `JWT_KEY` должен быть не короче 32 символов.
> ⚠️ `DB_PASSWORD` должен совпадать с паролем, указанным внутри `DB_CONNECTION_STRING`.

#### Хотите подключить свою базу данных вместо встроенной?

Замените всю строку `DB_CONNECTION_STRING` на адрес вашей БД:

```env
DB_CONNECTION_STRING=Server=my-external-db.com,1433;Database=TaskTrackerDb;User Id=myuser;Password=MyPassword;TrustServerCertificate=True
```

В этом случае сервис `db` из `docker-compose.yml` можно не запускать — см. шаг 3.

### 3. Запустите backend

**Со встроенной базой данных (по умолчанию):**

```bash
docker compose up -d --build
```

**С собственной внешней базой данных** (без контейнера `db`):

```bash
docker compose up -d --build api caddy
```

Дождитесь запуска (30–60 секунд). Проверить статус:

```bash
docker compose logs -f api
```

Когда увидите в логах `Создан админ ...` — backend готов и админ создан с данными из вашего `.env`.

API доступно по адресу: `http://localhost` (или `https://ваш-домен`, если настроен `DOMAIN`, см. ниже).

### 4. HTTPS на реальном сервере (не localhost)

Если деплоите на сервер с публичным доменом:

1. Направьте A-запись домена на IP вашего сервера.
2. В `.env` укажите:
```env
   DOMAIN=api.yourdomain.com
   ACME_EMAIL=you@yourdomain.com
```
3. Откройте порты **80** и **443** на сервере (firewall / security group облака).
4. Запустите:
```bash
   docker compose up -d --build
```

Caddy автоматически получит SSL-сертификат от Let's Encrypt и будет поддерживать его актуальным. API станет доступен по `https://api.yourdomain.com`.

Для локального запуска без домена просто оставьте `DOMAIN` пустым — всё будет работать по `http://localhost`.

---

## Desktop-клиент (Windows)

### Скачать готовую сборку

Скачайте архив с последним релизом со страницы [Releases](https://github.com/yourname/tasktracker/releases), распакуйте и запустите `TaskTracker.Maui.exe`.

> Установка .NET runtime **не требуется** — всё включено в архив.

### Настройка адреса API

После первого запуска рядом с `TaskTracker.Maui.exe` автоматически появится файл `config.json`:

```json
{
  "ApiBaseUrl": "http://localhost:8080/"
}
```

Если ваш backend запущен на другом адресе или другой машине — откройте `config.json` в любом текстовом редакторе, измените адрес (например, `http://192.168.1.10/` или `https://api.yourdomain.com/`) и перезапустите приложение. **Пересборка не требуется.**

## Mobile-клиент (Android / iOS)

На мобильных платформах файла рядом с `.exe` не существует — адрес сервера меняется через экран настроек внутри самого приложения:

1. Откройте приложение.
2. Перейдите в **Настройки** (иконка шестерёнки на экране входа).
3. Введите адрес вашего сервера, например `http://192.168.1.10:8080/` или `https://api.yourdomain.com/`.
4. Сохраните и перезапустите приложение.

> Если backend и телефон находятся в одной локальной сети, используйте локальный IP машины с backend (например, `192.168.1.10`), а не `localhost` — на телефоне `localhost` означает сам телефон.

### Войдите в систему

Используйте учётные данные из вашего `.env`:

| Поле     | Значение                            |
|----------|--------------------------------------|
| Email    | значение `ADMIN_EMAIL` из `.env`    |
| Password | значение `ADMIN_PASSWORD` из `.env` |

---

## Роли пользователей

| Роль                  | Возможности                                              |
|-----------------------|-----------------------------------------------------------|
| **Admin**              | Полный доступ ко всему, управление пользователями          |
| **ChiefProjectManager**| Все проекты, создание проектов, назначение менеджеров      |
| **ProjectManager**     | Только свои проекты, создание задач, управление командой    |
| **Worker**             | Только свои задачи, смена статуса назначенных задач         |

---

## Управление контейнерами

```bash
# Запустить
docker compose up -d

# Остановить (данные сохраняются)
docker compose down

# Остановить и удалить все данные (включая базу данных!)
docker compose down -v

# Посмотреть логи API
docker compose logs -f api

# Посмотреть логи базы данных
docker compose logs -f db

# Посмотреть логи Caddy (HTTPS)
docker compose logs -f caddy

# Пересобрать после изменений в коде
docker compose up -d --build
```

---

## Сборка из исходников

### Backend

Требуется только Docker Desktop:

```bash
docker compose up -d --build
```

### Desktop-клиент (Windows)

**Требования:**
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10)
- Windows 10 версии 19041 или новее
- Рабочая нагрузка `.NET MAUI` (`dotnet workload install maui-windows`)

> Если SDK обновился до новой feature-band версии и появляются ошибки про несовместимые версии рантайма пакетов (`NU1102` для `Microsoft.NETCore.App.Runtime.*`), выполните `dotnet workload update`, затем `dotnet workload install maui-windows android ios maccatalyst`, и при необходимости очистите кэш сборки: `find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +`.

```bash
dotnet publish TaskTracker.Maui/TaskTracker.Maui.csproj \
  -f net10.0-windows10.0.19041.0 \
  -c Release \
  -p:TargetFrameworks=net10.0-windows10.0.19041.0 \
  -p:WindowsPackageType=None \
  -p:SelfContained=true \
  -p:RuntimeIdentifier=win-x64 \
  -o ./publish/windows
```

> Параметр `-p:TargetFrameworks=net10.0-windows10.0.19041.0` обязателен — проект мультитаргетный (Android/iOS/MacCatalyst/Windows), и без него restore попытается резолвить зависимости сразу для всех платформ, что приводит к ошибкам поиска несуществующих пакетов.

Готовый `.exe` будет в папке `publish/windows`.

Для релиза на GitHub заархивируйте папку:

```powershell
Compress-Archive -Path ./publish/windows/* -DestinationPath TaskTracker-win-x64.zip
```

Загрузите `TaskTracker-win-x64.zip` в GitHub Releases — пользователи смогут скачать и запустить без каких-либо установок.

### Mobile-клиент (Android)

```bash
dotnet publish TaskTracker.Maui/TaskTracker.Maui.csproj \
  -f net10.0-android \
  -c Release \
  -o ./publish/android
```

### Mobile-клиент (iOS / macOS) — требуется macOS с Xcode

```bash
dotnet publish TaskTracker.Maui/TaskTracker.Maui.csproj \
  -f net10.0-ios \
  -c Release \
  -o ./publish/ios
```

---

## Структура проекта

tasktracker/
├── TaskTracker.WebApi/             # ASP.NET Core Web API
├── TaskTracker.Application/        # Use cases, интерфейсы
├── TaskTracker.Domain/             # Доменные сущности
├── TaskTracker.Infrastructure/     # EF Core, репозитории, сервисы
├── TaskTracker.Maui/               # .NET MAUI клиент (Windows/Android/iOS)
├── docker-compose.yml
├── Caddyfile
├── .env.example
└── README.md

---

## Переменные окружения

| Переменная               | Описание                                                                  |
|---------------------------|----------------------------------------------------------------------------|
| `DB_CONNECTION_STRING`   | Полная строка подключения к БД (своя или встроенная в docker-compose)    |
| `DB_PASSWORD`            | Пароль SA для встроенного контейнера MS SQL Server                       |
| `JWT_KEY`                | Секретный ключ для подписи JWT (мин. 32 символа)                         |
| `ADMIN_EMAIL`            | Email администратора, создаваемого автоматически при первом запуске      |
| `ADMIN_PASSWORD`         | Пароль администратора, создаваемого автоматически при первом запуске     |
| `DOMAIN`                 | Домен для автоматического HTTPS через Let's Encrypt (необязательно)      |
| `ACME_EMAIL`             | Email для регистрации сертификата Let's Encrypt (необязательно)          |
| `API_PORT`               | Порт, на котором публикуется API (по умолчанию 80)                       |

---

## Возможные проблемы

**Контейнер `db` не запускается**
Проверьте, что `DB_PASSWORD` соответствует требованиям сложности MS SQL Server (заглавные + строчные буквы + цифры + спецсимволы).

**Контейнер `api` не может подключиться к БД**
БД стартует дольше API. Подождите 30–60 секунд и проверьте `docker compose logs db`. Если проблема повторяется — `docker compose restart api`.

**Не получается HTTPS-сертификат**
Убедитесь, что домен действительно указывает на IP сервера (`nslookup ваш-домен`), и что порты 80/443 открыты во внешнем firewall. Caddy не сможет получить сертификат, если порт 80 недоступен снаружи.

**`TaskTracker.Maui.exe` не запускается**
Убедитесь, что Windows 10 обновлена до версии 19041 (May 2020 Update) или новее.

**При сборке Dockerfile появляется `NETSDK1045: does not support targeting .NET 10.0`**
В `Dockerfile` используются образы под устаревшую версию SDK. Убедитесь, что в `FROM`-строках указано `mcr.microsoft.com/dotnet/sdk:10.0` и `mcr.microsoft.com/dotnet/aspnet:10.0`, а не `9.0`.

**При сборке Dockerfile ошибка `Unable to find fallback package folder ... NuGetPackages`**
Это означает, что папки `bin`/`obj` с Windows-путями попали в Docker build context. Создайте/проверьте `.dockerignore` в корне репозитория со строками `**/bin/`, `**/obj/`, `**/.vs/`, и пересоберите без кэша: `docker compose build --no-cache`.

**MAUI-приложение крашится на старте с `WinRT.Runtime` / `InvalidOperationException: Operation is not valid due to the current state of the object`**
Причина — вызов MAUI Essentials API (`FileSystem.OpenAppPackageFileAsync`, `Preferences`) внутри `MauiProgram.CreateMauiApp()` до полной инициализации платформы. Используйте чтение `appsettings.json` через `Assembly.GetManifestResourceStream` (`EmbeddedResource` вместо `MauiAsset`), а доступ к `Preferences` на мобильных платформах выполняйте лениво — например, через `DelegatingHandler`, срабатывающий уже после запуска приложения, а не при старте `MauiProgram`.

**Клиент не подключается к API**
- Проверьте, что Docker-контейнеры работают: `docker compose ps`.
- На десктопе проверьте `config.json` рядом с `.exe` — указан ли там правильный адрес.
- На мобильном устройстве проверьте экран **Настройки** — указан ли правильный адрес, и используется ли реальный IP сервера, а не `localhost`.
- Убедитесь, что устройство (телефон/ПК) и сервер находятся в одной сети, либо что сервер доступен публично по домену с HTTPS.
