# Выложить motores2 в облако

## Вариант 1: GitHub (Рекомендуется)

### Шаг 1: Создать репозиторий на GitHub
1. Заходишь на https://github.com/new
2. Называешь репозиторий: `motores2` или `swarm-survivor`
3. Выбираешь Private (если не хочешь публики)
4. Нажимаешь "Create repository"

### Шаг 2: Добавить remote и push

```bash
cd "E:\..davinci_learn\My Game\motores2"

# Добавляем GitHub как удалённый репозиторий
git remote add origin https://github.com/YOUR_USERNAME/motores2.git

# Переименовываем ветку в main (GitHub стандарт)
git branch -M main

# Выгружаем на GitHub
git push -u origin main
```

**Замени YOUR_USERNAME на своё имя пользователя GitHub!**

### Шаг 3: Готово!

Проект теперь на GitHub:
```
https://github.com/YOUR_USERNAME/motores2
```

---

## Вариант 2: Unity Cloud (Unity Teams)

Если у тебя есть Unity Pro:

1. Открой motores2 в Unity
2. Window → Unity Cloud → Sign In
3. Следуй инструкциям на экране
4. Version Control будет автоматически загружать файлы

---

## Вариант 3: GitLab

Если предпочитаешь GitLab:

```bash
git remote add origin https://gitlab.com/YOUR_USERNAME/motores2.git
git push -u origin main
```

---

## После загрузки

### Клонировать проект на другом ПК:

```bash
git clone https://github.com/YOUR_USERNAME/motores2.git
cd motores2
# Открыть в Unity
```

### Обновить изменения:

```bash
git add .
git commit -m "Describe changes here"
git push
```

### Скачать последние изменения:

```bash
git pull
```

---

## Что в репозитории сейчас

```
✅ Assets/Scripts/
   - 10 Symbol Door файлов (ядро)
   - WaveManager.cs (интеграция)
   - Все остальное (враги, игрок, UI)

✅ Assets/Scenes/
   - Level1, Level2, MainMenu, SplashScreen

✅ Assets/Editor/
   - Scene setup scripts

✅ Documentation/
   - README_SYMBOL_DOOR.md
   - SYMBOL_DOOR_USAGE.md
   - SYMBOL_DOOR_DESIGN_v4.md
   - И ещё 5 доков
```

---

## Размер репозитория

```
Total: ~200 MB (в основном Assets/TextMesh Pro)
Code: ~500 KB (символ дверь + весь скрипты)
```

---

## .gitignore (уже добавлен)

```
Library/
Temp/
Builds/
.vs/
.vscode/
obj/
bin/
*.o
```

Это исключает большие файлы, которые регенерирует Unity.

---

## Полезные команды

```bash
# Посмотреть статус
git status

# Посмотреть историю
git log --oneline

# Откатить последний коммит
git reset --soft HEAD~1

# Создать ветку для экспериментов
git checkout -b feature/new-puzzle

# Слить ветку в main
git checkout main
git merge feature/new-puzzle
```

---

## Нужна помощь?

- GitHub docs: https://docs.github.com/en
- Git docs: https://git-scm.com/doc
- Unity + Git: https://docs.unity3d.com/Manual/ProjectFolder.html

---

**Выбери вариант 1 (GitHub) — это самый стандартный и удобный!** ✨
