using System.Collections.Generic;
using System.Numerics;
using TurboHedgehogForms.Entities;

namespace TurboHedgehogForms.Game
{
    /// <summary>
    /// Генерирует уровни. Уровни длинные и с несколькими путями:
    /// нижний путь проще, верхний требует прыжков/скорости и дает больше колец.
    /// </summary>
    public static class LevelFactory
    {
        public static void Build(
            LevelId id,
            List<Platform> platforms,
            List<Ring> rings,
            List<EnemyPatrol> enemies,
            out FinishFlag finish,
            out Boss? boss,
            out Vector2 spawn)
        {
            platforms.Clear();
            rings.Clear();
            enemies.Clear();
            boss = null;

            spawn = new Vector2(80, 200);

            switch (id)
            {
                case LevelId.Act1:
                    BuildAct1(platforms, rings, enemies, out finish);
                    break;
                case LevelId.Act2:
                    BuildAct2(platforms, rings, enemies, out finish);
                    break;
                case LevelId.Act3_Boss:
                    BuildAct3Boss(platforms, rings, enemies, out finish, out boss);
                    break;
                default:
                    BuildAct1(platforms, rings, enemies, out finish);
                    break;
            }
        }

        private static void BuildAct1(List<Platform> p, List<Ring> r, List<EnemyPatrol> e, out FinishFlag finish)
        {
            // Длина ~ 4200
            p.Add(new Platform(new Vector2(0, 460), new Vector2(4400, 80))); // земля

            // Нижний путь: длинный и “пологий”
            p.Add(new Platform(new Vector2(400, 420), new Vector2(400, 20)));
            p.Add(new Platform(new Vector2(900, 400), new Vector2(600, 20)));
            p.Add(new Platform(new Vector2(1700, 430), new Vector2(500, 20)));
            p.Add(new Platform(new Vector2(2400, 410), new Vector2(800, 20)));

            // Верхний путь: требует прыжков, дает много колец
            p.Add(new Platform(new Vector2(350, 320), new Vector2(220, 20)));
            p.Add(new Platform(new Vector2(650, 280), new Vector2(220, 20)));
            p.Add(new Platform(new Vector2(980, 250), new Vector2(260, 20)));
            p.Add(new Platform(new Vector2(1360, 260), new Vector2(260, 20)));
            p.Add(new Platform(new Vector2(1780, 240), new Vector2(260, 20)));
            p.Add(new Platform(new Vector2(2200, 270), new Vector2(260, 20)));
            p.Add(new Platform(new Vector2(2620, 250), new Vector2(280, 20)));
            p.Add(new Platform(new Vector2(3100, 290), new Vector2(300, 20)));

            // Перемычки/альтернативы (множество путей)
            p.Add(new Platform(new Vector2(1200, 340), new Vector2(140, 20)));
            p.Add(new Platform(new Vector2(2000, 330), new Vector2(140, 20)));
            p.Add(new Platform(new Vector2(2900, 360), new Vector2(160, 20)));

            // Кольца на верхнем пути
            for (int i = 0; i < 20; i++)
                r.Add(new Ring(new Vector2(380 + i * 30, 290)));

            for (int i = 0; i < 16; i++)
                r.Add(new Ring(new Vector2(1050 + i * 30, 220)));

            // Кольца на нижнем пути
            for (int i = 0; i < 12; i++)
                r.Add(new Ring(new Vector2(950 + i * 30, 370)));

            // Враги
            e.Add(new EnemyPatrol(new Vector2(980, 380), 900, 1500));
            e.Add(new EnemyPatrol(new Vector2(1900, 410), 1700, 2200));
            e.Add(new EnemyPatrol(new Vector2(2800, 390), 2400, 3200));

            finish = new FinishFlag(new Vector2(4100, 396));
        }

        private static void BuildAct2(List<Platform> p, List<Ring> r, List<EnemyPatrol> e, out FinishFlag finish)
        {
            // Длина ~ 5200. Более “вертикально” и с альтернативными дорожками.
            p.Add(new Platform(new Vector2(0, 460), new Vector2(5400, 80)));

            // Нижний маршрут
            p.Add(new Platform(new Vector2(500, 430), new Vector2(450, 20)));
            p.Add(new Platform(new Vector2(1100, 420), new Vector2(550, 20)));
            p.Add(new Platform(new Vector2(1850, 440), new Vector2(600, 20)));
            p.Add(new Platform(new Vector2(2700, 420), new Vector2(700, 20)));
            p.Add(new Platform(new Vector2(3700, 440), new Vector2(700, 20)));

            // Верхний маршрут (серия островков)
            float x = 600;
            float y = 320;
            for (int i = 0; i < 12; i++)
            {
                p.Add(new Platform(new Vector2(x, y), new Vector2(180, 18)));
                x += 240;
                y += (i % 3 == 0) ? -35 : 25;
                y = MathUtil.Clamp(y, 200, 360);
            }

            // “Средний” маршрут
            p.Add(new Platform(new Vector2(1600, 330), new Vector2(260, 18)));
            p.Add(new Platform(new Vector2(2000, 300), new Vector2(260, 18)));
            p.Add(new Platform(new Vector2(2400, 320), new Vector2(260, 18)));
            p.Add(new Platform(new Vector2(2900, 300), new Vector2(320, 18)));
            p.Add(new Platform(new Vector2(3400, 320), new Vector2(320, 18)));

            // Кольца: награда за верхний/средний
            for (int i = 0; i < 28; i++)
                r.Add(new Ring(new Vector2(650 + i * 35, 260)));

            for (int i = 0; i < 14; i++)
                r.Add(new Ring(new Vector2(1650 + i * 30, 300)));

            // Враги
            e.Add(new EnemyPatrol(new Vector2(1250, 400), 1100, 1650));
            e.Add(new EnemyPatrol(new Vector2(3000, 400), 2700, 3400));
            e.Add(new EnemyPatrol(new Vector2(4100, 420), 3700, 4400));

            finish = new FinishFlag(new Vector2(5100, 396));
        }

        private static void BuildAct3Boss(
            List<Platform> p, List<Ring> r, List<EnemyPatrol> e,
            out FinishFlag finish, out Boss? boss)
        {
            // Длина ~ 3600 + арена босса
            p.Add(new Platform(new Vector2(0, 460), new Vector2(3800, 80)));

            // “Подводка” к боссу (несколько путей)
            p.Add(new Platform(new Vector2(400, 410), new Vector2(450, 20)));
            p.Add(new Platform(new Vector2(950, 360), new Vector2(250, 20)));
            p.Add(new Platform(new Vector2(1250, 320), new Vector2(250, 20)));
            p.Add(new Platform(new Vector2(1650, 360), new Vector2(350, 20)));
            p.Add(new Platform(new Vector2(2200, 410), new Vector2(500, 20)));

            // Верхняя ветка
            p.Add(new Platform(new Vector2(900, 250), new Vector2(220, 18)));
            p.Add(new Platform(new Vector2(1180, 220), new Vector2(220, 18)));
            p.Add(new Platform(new Vector2(1500, 240), new Vector2(220, 18)));
            p.Add(new Platform(new Vector2(1820, 220), new Vector2(220, 18)));

            for (int i = 0; i < 18; i++)
                r.Add(new Ring(new Vector2(950 + i * 35, 190)));

            // Арена босса
            p.Add(new Platform(new Vector2(2900, 420), new Vector2(800, 20))); // пол арены
            // стены арены (чтобы игрок не убегал)
            p.Add(new Platform(new Vector2(2880, 260), new Vector2(20, 200)));
            p.Add(new Platform(new Vector2(3700, 260), new Vector2(20, 200)));

            boss = new Boss(new Vector2(3300, 350));
            finish = new FinishFlag(new Vector2(3600, 356)); // “капсула/флаг” появится после босса (логика в GameWorld)
        }
    }
}