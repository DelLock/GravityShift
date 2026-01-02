using System.Collections.Generic;
using System.Numerics;
using TurboHedgehogForms.Entities;

namespace TurboHedgehogForms.Game
{
    public static class LevelFactory
    {
        public static void Build(
            LevelId id,
            List<Platform> platforms,
            List<RampPlatform> ramps,
            List<SpeedPad> speedPads,
            List<Ring> rings,
            List<EnemyPatrol> enemies,
            out FinishFlag finish,
            out Boss? boss,
            out Vector2 spawn)
        {
            platforms.Clear();
            ramps.Clear();
            speedPads.Clear();
            rings.Clear();
            enemies.Clear();
            boss = null;

            spawn = new Vector2(80, 200);

            switch (id)
            {
                case LevelId.Act1:
                    BuildAct1(platforms, ramps, speedPads, rings, enemies, out finish);
                    break;
                case LevelId.Act2:
                    BuildAct2(platforms, ramps, speedPads, rings, enemies, out finish);
                    break;
                case LevelId.Act3_Boss:
                    BuildAct3Boss(platforms, ramps, speedPads, rings, enemies, out finish, out boss);
                    break;
                default:
                    BuildAct1(platforms, ramps, speedPads, rings, enemies, out finish);
                    break;
            }
        }

        private static void BuildAct1(
            List<Platform> p,
            List<RampPlatform> ramps,
            List<SpeedPad> pads,
            List<Ring> r,
            List<EnemyPatrol> e,
            out FinishFlag finish)
        {
            p.Add(new Platform(new Vector2(0, 460), new Vector2(6200, 90), PlatformKind.Ground));

            p.Add(new Platform(new Vector2(600, 270), new Vector2(720, 44), PlatformKind.Ground));
            p.Add(new Platform(new Vector2(1700, 240), new Vector2(820, 44), PlatformKind.Ground));
            p.Add(new Platform(new Vector2(2900, 280), new Vector2(900, 44), PlatformKind.Ground));
            p.Add(new Platform(new Vector2(4200, 250), new Vector2(780, 44), PlatformKind.Ground));

            ramps.Add(RampPlatform.Up(new Vector2(420, 440), width: 260, height: 120));
            ramps.Add(RampPlatform.Down(new Vector2(900, 320), width: 280, height: 140));
            ramps.Add(RampPlatform.Up(new Vector2(1400, 420), width: 300, height: 140));

            // momentум-ловушка
            p.Add(new Platform(new Vector2(2100, 430), new Vector2(90, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(2230, 410), new Vector2(90, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(2360, 390), new Vector2(90, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(2490, 370), new Vector2(90, 18), PlatformKind.Floating));

            // гарантированная цепочка прыжков (достижима)
            p.Add(new Platform(new Vector2(800, 210), new Vector2(170, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1040, 185), new Vector2(170, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1280, 160), new Vector2(170, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1520, 185), new Vector2(190, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1780, 220), new Vector2(220, 18), PlatformKind.Floating));

            // speed-section (псевдо-петля)
            pads.Add(new SpeedPad(new Vector2(2550, 410), new Vector2(420, 50), pushX: 900f, pushY: -180f));

            // после speed-section — сложнее сохранить скорость
            p.Add(new Platform(new Vector2(3050, 420), new Vector2(140, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(3260, 400), new Vector2(120, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(3460, 420), new Vector2(140, 18), PlatformKind.Floating));

            ramps.Add(RampPlatform.Down(new Vector2(3700, 260), width: 420, height: 200));

            // бонус путь
            p.Add(new Platform(new Vector2(4100, 160), new Vector2(220, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(4400, 130), new Vector2(220, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(4700, 160), new Vector2(220, 18), PlatformKind.Floating));

            for (int i = 0; i < 12; i++)
                r.Add(new Ring(new Vector2(840 + i * 28, 180)));
            for (int i = 0; i < 10; i++)
                r.Add(new Ring(new Vector2(2570 + i * 28, 375)));
            for (int i = 0; i < 12; i++)
                r.Add(new Ring(new Vector2(4140 + i * 28, 130)));

            e.Add(new EnemyPatrol(new Vector2(1200, 440), 1050, 1550));
            e.Add(new EnemyPatrol(new Vector2(2800, 440), 2700, 3000));
            e.Add(new EnemyPatrol(new Vector2(4800, 440), 4550, 5050));

            finish = new FinishFlag(new Vector2(5950, 382));
        }

        private static void BuildAct2(
            List<Platform> p,
            List<RampPlatform> ramps,
            List<SpeedPad> pads,
            List<Ring> r,
            List<EnemyPatrol> e,
            out FinishFlag finish)
        {
            p.Add(new Platform(new Vector2(0, 460), new Vector2(7400, 90), PlatformKind.Ground));

            p.Add(new Platform(new Vector2(500, 260), new Vector2(800, 44), PlatformKind.Ground));
            p.Add(new Platform(new Vector2(1600, 280), new Vector2(900, 44), PlatformKind.Ground));
            p.Add(new Platform(new Vector2(3000, 240), new Vector2(1000, 44), PlatformKind.Ground));
            p.Add(new Platform(new Vector2(4700, 270), new Vector2(900, 44), PlatformKind.Ground));

            ramps.Add(RampPlatform.Down(new Vector2(320, 300), width: 380, height: 160));
            ramps.Add(RampPlatform.Up(new Vector2(900, 440), width: 420, height: 170));
            ramps.Add(RampPlatform.Down(new Vector2(1600, 290), width: 460, height: 200));
            ramps.Add(RampPlatform.Up(new Vector2(2300, 440), width: 420, height: 170));

            // гарантированная цепочка прыжков
            p.Add(new Platform(new Vector2(1200, 190), new Vector2(190, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1480, 170), new Vector2(190, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1760, 190), new Vector2(190, 18), PlatformKind.Floating));

            // моментум сложнее
            p.Add(new Platform(new Vector2(2650, 390), new Vector2(120, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(2850, 360), new Vector2(120, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(3050, 330), new Vector2(120, 18), PlatformKind.Floating));
            ramps.Add(RampPlatform.Up(new Vector2(3250, 440), width: 300, height: 150));

            pads.Add(new SpeedPad(new Vector2(3900, 410), new Vector2(520, 50), pushX: 1000f, pushY: -220f));

            // после speed-section — точнее прыжки
            p.Add(new Platform(new Vector2(4480, 420), new Vector2(120, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(4680, 400), new Vector2(120, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(4880, 380), new Vector2(120, 18), PlatformKind.Floating));

            // бонус путь
            p.Add(new Platform(new Vector2(5400, 160), new Vector2(240, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(5720, 130), new Vector2(240, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(6040, 160), new Vector2(240, 18), PlatformKind.Floating));

            for (int i = 0; i < 14; i++)
                r.Add(new Ring(new Vector2(1230 + i * 28, 160)));
            for (int i = 0; i < 12; i++)
                r.Add(new Ring(new Vector2(3920 + i * 28, 375)));
            for (int i = 0; i < 12; i++)
                r.Add(new Ring(new Vector2(5440 + i * 28, 130)));

            e.Add(new EnemyPatrol(new Vector2(1900, 440), 1600, 2400));
            e.Add(new EnemyPatrol(new Vector2(4300, 440), 3900, 4400));
            e.Add(new EnemyPatrol(new Vector2(6200, 440), 5900, 6500));

            finish = new FinishFlag(new Vector2(7100, 382));
        }

        private static void BuildAct3Boss(
            List<Platform> p,
            List<RampPlatform> ramps,
            List<SpeedPad> pads,
            List<Ring> r,
            List<EnemyPatrol> e,
            out FinishFlag finish,
            out Boss? boss)
        {
            // ====== КЛЮЧЕВАЯ ПРАВКА: ДО БОССА МОЖНО ДОЙТИ ПО ЗЕМЛЕ ======
            // Земля до арены (не единая плита, но с гарантированным непрерывным маршрутом)
            p.Add(new Platform(new Vector2(0, 460), new Vector2(5200, 90), PlatformKind.Ground));

            // Склоны до арены: не блокируют путь, а дают рельеф
            ramps.Add(RampPlatform.Up(new Vector2(260, 440), width: 360, height: 160));
            ramps.Add(RampPlatform.Down(new Vector2(780, 280), width: 360, height: 160));

            // Небольшая speed-section перед ареной
            pads.Add(new SpeedPad(new Vector2(1320, 410), new Vector2(420, 50), pushX: 950f, pushY: -160f));

            // Гарантированная цепочка платформ (бонус маршрут), но НЕ обязательная
            p.Add(new Platform(new Vector2(1040, 210), new Vector2(200, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1320, 185), new Vector2(200, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(1600, 210), new Vector2(200, 18), PlatformKind.Floating));

            // Моментум-трап перед ареной (не ломает прохождение — есть земля снизу)
            p.Add(new Platform(new Vector2(2100, 420), new Vector2(120, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(2300, 400), new Vector2(120, 18), PlatformKind.Floating));
            p.Add(new Platform(new Vector2(2500, 420), new Vector2(120, 18), PlatformKind.Floating));

            // ====== Арена босса ======
            p.Add(new Platform(new Vector2(3600, 420), new Vector2(900, 22), PlatformKind.Ground)); // пол арены
            p.Add(new Platform(new Vector2(3580, 260), new Vector2(20, 200), PlatformKind.Ground)); // левая стенка
            p.Add(new Platform(new Vector2(4480, 260), new Vector2(20, 200), PlatformKind.Ground)); // правая стенка

            boss = new Boss(new Vector2(4040, 350));

            // Кольца до босса (по земле и на бонус пути)
            for (int i = 0; i < 10; i++)
                r.Add(new Ring(new Vector2(520 + i * 30, 420)));
            for (int i = 0; i < 12; i++)
                r.Add(new Ring(new Vector2(1060 + i * 28, 180)));

            // Финиш после босса (разблокируется логикой в GameWorld)
            finish = new FinishFlag(new Vector2(4380, 356));
        }
    }
}