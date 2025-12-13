using System;
using System.Drawing;
using AstralScavenger.Models.Entities;

namespace AstralScavenger.Models.Logic;

public class PlayerLogic
{
    public void SetPlayerDirectionAndMove(Player player, int dx, int dy, int width, int height, int playerSize)
    {
        var totalDx = dx;
        var totalDy = dy;

        if (totalDx != 0 || totalDy != 0)
            player.TargetRotation = (float)Math.Atan2(-totalDy, -totalDx) - (float)(Math.PI / 2);

        var newX = player.Position.X + totalDx * player.Speed;
        var newY = player.Position.Y + totalDy * player.Speed;

        if (newX >= 0 && newX <= width - playerSize)
            player.Position = new Point((int)newX, player.Position.Y);

        if (newY >= 0 && newY <= height - playerSize)
            player.Position = new Point(player.Position.X, (int)newY);
    }

    public void UpdatePlayerRotation(Player player)
    {
        if (player.CurrentRotation != player.TargetRotation)
        {
            float diff = player.TargetRotation - player.CurrentRotation;
            if (diff > Math.PI) diff -= 2 * (float)Math.PI;
            if (diff < -Math.PI) diff += 2 * (float)Math.PI;
            player.CurrentRotation += diff * player.RotationSpeed;
            if (Math.Abs(diff) < 0.01f)
                player.CurrentRotation = player.TargetRotation;
        }
    }
}