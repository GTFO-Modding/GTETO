using System;
using UnityEngine;

public sealed class EditorContentColorScope : IDisposable
{
    private readonly Color m_color;
    private readonly Color m_oldColor;

    public EditorContentColorScope(Color color)
    {
        this.m_color = color;
        this.m_oldColor = GUI.contentColor;
        GUI.contentColor = this.m_color;
    }

    public void Dispose()
    {
        GUI.contentColor = this.m_oldColor;
    }
}