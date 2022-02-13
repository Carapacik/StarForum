# StarForum

## Features

### Roles

- User
- Administrator

### The possibilities of the role:

#### User
- Login and registration on the forum
- Change the description, status and image of your profile
- Creating, editing (your) and archive (your) topic
- Creating and editing (your) reply to topics
- Global search and category search
- Activity rating

#### Administrator:
- All **User** features
- Changing the profile of other users (including changing their login)
- View all users on the site (sorted by registration date, by login, by activity rating, by email)
- Create and edit categories for topics
- Edit and close all topics
- Edit and delete answers to all topics

## Возможности

### Роли

- Пользователь
- Администратор

### Возможности ролей:

#### Пользователь
- Вход и регистрация на форуме
- Изменение описания, статуса, изображения своего профиля
- Создание, редактирование(своей) и закрытие(своей) темы
- Создание и редактирование(своего) ответа на темы
- Глобальный поиск и поиск по категориям
- Наличие рейтинга активности

#### Администратор:
- Все возможности **Пользователя**
- Изменение профиля других пользователей(в том числе смена их логина)
- Просмотр всех пользователей на сайте(с сортировкой по дате регистрации, по логину, по рейтингу активности, по почте)
- Создание и редактирование категорий для тем
- Редактирование и закрытие всех тем
- Редактирование и удаление ответов на все темы

## Preparing for launch

- Requires **.NET 6** version or higher
- Requires **MS SQL Server**

In the file **```/Star Forum.WebAPI/appsettings.json```** specify the server and database in the *DefaultConnection* string

Go to the folder **```/ Star Forum.Infrastructure```** and [creating a database](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations ) based on migrations depending on the IDE:
- Visual Studio:
```
Update-Database
```
- .NET CLI
```
dotnet ef database update
```

## Подготовка к запуску

- Необходим **.NET 6** версии или выше
- Необходим **MS SQL Server**

В файле **```/StarForum.WebApi/appsettings.json```** указываем сервер и базу данных в строку *DefaultConnection*

Переходим в папку **```/StarForum.Infrastructure```** и [создаём базу данных](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations) на основе миграций в зависимости от IDE:
- Visual Studio:
```
Update-Database
```
- .NET CLI
```
dotnet ef database update
```