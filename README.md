# 🛂 КПП — Контроль гостей (Blazor WebAssembly)

Blazor WebAssembly версия системы КПП, полностью воспроизводящая функционал
оригинального JavaScript-проекта на C# + Razor-компонентах.

---

## 📁 Структура проекта

```
KppBlazor/
├── Models/
│   └── Models.cs              # GuestItem, RoleItem, UserItem, AuthInfo, ...
├── Services/
│   ├── ApiService.cs          # HTTP-клиент (аналог api.js)
│   └── AppState.cs            # Глобальное состояние + события (аналог AUTH/VIEW)
├── Pages/
│   ├── Login.razor            # Страница входа
│   ├── History.razor          # История посещений + фильтры + модалка
│   ├── Register.razor         # Регистрация / заявка гостя
│   ├── Users.razor            # Управление пользователями (admin)
│   └── Roles.razor            # Управление ролями (admin)
├── Shared/
│   └── Sidebar.razor          # Боковая панель навигации + статистика
├── App.razor                  # Корневой компонент-роутер
├── Program.cs                 # DI + Blazor WASM bootstrap
├── StringExtensions.cs        # Вспомогательный метод NullIfEmpty()
├── KppBlazor.csproj           # .NET 8 Blazor WASM project file
└── wwwroot/
    ├── index.html             # Точка входа HTML
    └── css/
        └── site.css           # Полные стили (перенесено из site.css)
```

---

## 🚀 Запуск

### Требования
- .NET 8 SDK: https://dotnet.microsoft.com/download

### Команды

```bash
cd KppBlazor

# Восстановить зависимости
dotnet restore

# Запустить dev-сервер (откроется браузер)
dotnet run
```

По умолчанию откроется `https://localhost:5001` (или `http://localhost:5000`).

Приложение будет проксировать API-запросы на бэкенд. Задайте адрес бэкенда
в `Program.cs`:

```csharp
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://your-backend-api.com") });
```

---

## 🔄 Соответствие JS → Blazor

| JS-файл        | Blazor-аналог              | Описание                              |
|----------------|---------------------------|---------------------------------------|
| `api.js`       | `Services/ApiService.cs`  | HTTP-клиент, AUTH, apiFetch           |
| `app.js`       | `App.razor`               | Маршрутизация VIEW, render()          |
| `utils.js`     | Встроено в компоненты     | fmtDT, todayStr, badge, DOM-хелперы   |
| `sidebar.js`   | `Shared/Sidebar.razor`    | Сайдбар + статистика                  |
| `login.js`     | `Pages/Login.razor`       | Форма авторизации                     |
| `history.js`   | `Pages/History.razor`     | Таблица, фильтры, сводка, модалка     |
| `register.js`  | `Pages/Register.razor`    | Форма регистрации гостей / заявки     |
| `users.js`     | `Pages/Users.razor`       | CRUD пользователей                    |
| `roles.js`     | `Pages/Roles.razor`       | CRUD ролей                            |
| `site.css`     | `wwwroot/css/site.css`    | Полный перенос стилей                 |

---

## ⚙️ Архитектурные решения

### Управление состоянием
Вместо глобальных JS-переменных (`AUTH`, `VIEW`, `ROLES_CACHE`) используется
`AppState` — singleton-сервис с событием `OnChange`. Компоненты подписываются
на это событие и вызывают `StateHasChanged()` для перерисовки.

```csharp
// AppState хранит:
AuthInfo? Auth          // текущий пользователь (аналог AUTH)
string CurrentView      // текущий раздел (аналог VIEW)
List<RoleItem> RolesCache  // кэш ролей
SidebarStats Stats      // счётчики для сайдбара
```

### API-клиент
`ApiService` — scoped-сервис, использующий `HttpClient`. Методы:
- `LoginAsync` — POST /api/auth/login, сохраняет токен в заголовки
- `GetGuestsAsync` — GET /api/guests с параметрами фильтрации
- `GuestEntryAsync` / `GuestExitAsync` — регистрация входа/выхода
- `GetRolesAsync`, `CreateRoleAsync`, `UpdateRoleAsync`, `DeleteRoleAsync`
- `GetUsersAsync`, `CreateUserAsync`, `UpdateUserAsync`, `DeleteUserAsync`

### Автообновление истории
В `History.razor` используется `System.Timers.Timer` (аналог `setInterval`)
для обновления таблицы каждые 30 секунд. Таймер останавливается при
уничтожении компонента (`IDisposable`).

### Сканирование паспорта (ИИ)
В `Register.razor` используется Blazor `InputFile` для загрузки изображения.
Файл конвертируется в Base64 и отправляется на `/api/ai/scan-passport`.
В текущей реализации показывается заглушка — подключите реальный эндпоинт.

---

## 🎨 UI / Дизайн

Стили полностью перенесены из оригинального `site.css` без изменений.
Тёмная тема с акцентами `#1565c0` (синий), `#7ecfff` (голубой), `#ffcc02`
(жёлтый). Адаптирован для Blazor: добавлены классы `.login-wrap`,
`.login-box`, `.table-msg`, `.table-err`.

---

## 🔌 Добавление бэкенда (ASP.NET Core)

Для fullstack-решения на одном стеке:

```bash
dotnet new blazorwasm --hosted -n KppApp
```

Это создаст:
- `KppApp.Client` — Blazor WASM (текущий код)
- `KppApp.Server` — ASP.NET Core с контроллерами `/api/...`
- `KppApp.Shared` — общие модели

Перенесите модели из `Models/Models.cs` в `Shared`, а бизнес-логику —
в `Server/Controllers/`.
