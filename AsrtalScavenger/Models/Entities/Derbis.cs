using System.Drawing;
using AstralScavenger.Models.States;

namespace AstralScavenger.Models.Entities;

public class Debris
{
    public Point Position { get; set; }
    public Point Velocity { get; set; } = new(0, 0);
    public int Size { get; set; } = 30;
    public bool IsCollectible { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool IsStatic { get; set; } = false;
    public DebrisType Type { get; set; } = DebrisType.Standard;

    public Rectangle Hitbox { get; private set; } = new(0, 0, 30, 30);

    public void UpdateHitbox()
    {
        int hitboxSize = (int)(Size * 0.8f);
        int offset = (Size - hitboxSize) / 2;

        Hitbox = new Rectangle(Position.X + offset, Position.Y + offset, hitboxSize, hitboxSize);
    }
}