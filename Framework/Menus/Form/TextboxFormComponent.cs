﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;

namespace Entoarox.Framework.Menus
{
    public class TextboxFormComponent : BaseFormComponent, IKeyboardSubscriber
    {
        protected static Texture2D Box;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        public bool Selected { get; set; }
        public event ValueChanged<string> Handler;
        public Func<TextboxFormComponent, FrameworkMenu, string, string> TabPressed;
        public Func<TextboxFormComponent, FrameworkMenu, string, string> EnterPressed;
        protected string _Value;
        protected Predicate<string> Validator = (e) => true;
        protected string OldValue;
        protected FrameworkMenu Menu;
        public TextboxFormComponent(Point position, ValueChanged<string> handler = null) : this(position, 75, null, handler)
        {

        }
        public TextboxFormComponent(Point position, Predicate<string> validator, ValueChanged<string> handler = null) : this(position, 75, validator, handler)
        {

        }
        public TextboxFormComponent(Point position, int width, ValueChanged<string> handler=null) : this(position, width, null, handler)
        {

        }
        public TextboxFormComponent(Point position, int width, Predicate<string> validator, ValueChanged<string> handler=null)
        {
            if(Box==null)
                Box=Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            SetScaledArea(new Rectangle(position.X, position.Y, width, Box.Height/Game1.pixelZoom));
            if (validator != null)
                Validator = validator;
            if (handler != null)
                Handler += handler;
            Value = "";
            OldValue = Value;
        }
        public override void FocusLost(IComponentCollection c, FrameworkMenu m)
        {
            Selected = false;
            m.TextboxActive = false;
            Game1.keyboardDispatcher.Subscriber = null;
            if (OldValue.Equals(Value))
                return;
            OldValue = Value;
            Handler?.Invoke(this, c, m, Value);
        }
        public override void FocusGained(IComponentCollection c, FrameworkMenu m)
        {
            Selected = true;
            m.TextboxActive = true;
            Menu = m;
            Game1.keyboardDispatcher.Subscriber = this;
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            bool flag = DateTime.Now.Millisecond % 1000 >= 500;
            string text = Value;
            b.Draw(Box, new Rectangle(Area.X + o.X, Area.Y + o.Y, Game1.pixelZoom * 4, Area.Height), new Rectangle(Game1.pixelZoom, 0, Game1.pixelZoom * 4, Area.Height), Color.White);
            b.Draw(Box, new Rectangle(Area.X+o.X + Game1.pixelZoom * 4, Area.Y+o.Y, Area.Width - Game1.pixelZoom * 8, Area.Height), new Rectangle(Game1.pixelZoom * 4, 0, 4, Area.Height), Color.White);
            b.Draw(Box, new Rectangle(Area.X+o.X + Area.Width - Game1.pixelZoom * 4, Area.Y+o.Y, Game1.pixelZoom * 4, Area.Height), new Rectangle(Box.Bounds.Width - Game1.pixelZoom * 4, 0, Game1.pixelZoom * 4, Area.Height), Color.White);
            Vector2 v;
            for (v = Game1.smallFont.MeasureString(text); v.X > Area.Width - Game1.pixelZoom*5; v = Game1.smallFont.MeasureString(text))
                text = text.Substring(1);
            if (flag && Selected)
                b.Draw(Game1.staminaRect, new Rectangle(Area.X+o.X + (int)(Game1.pixelZoom*3.5) + (int)v.X, Area.Y+o.Y + 8, Game1.pixelZoom/2, (int)Game1.smallFont.MeasureString("|").Y), Game1.textColor);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(Area.X+o.X + Game1.pixelZoom*4, Area.Y+o.Y+Game1.pixelZoom*3), Game1.textColor);
        }
        public void RecieveTextInput(char inputChar)
        {
            Game1.playSound("cowboy_monsterhit");
            RecieveTextInput(inputChar.ToString());
        }
        public void RecieveTextInput(string text)
        {
            if (Disabled || !Validator(text))
                return;
            Value = Value + text;
        }
        public void RecieveCommandInput(char c)
        {
            if (Disabled)
                return;
            switch((int)c)
            {
                case 8:
                    if (Value.Length <= 0)
                        return;
                    Value = Value.Substring(0, Value.Length - 1);
                    Game1.playSound("tinyWhip");
                    return;
                case 9:
                    if (TabPressed != null)
                    {
                        Value = TabPressed(this, Menu, Value);
                        return;
                    }
                    bool Next = false;
                    IInteractiveMenuComponent first=null;
                    foreach(IInteractiveMenuComponent imc in Menu.InteractiveComponents)
                    {
                        if (first == null && imc is TextboxFormComponent)
                            first = imc;
                        if (imc == this)
                        {
                            Next = true;
                            continue;
                        }
                        if (Next && imc is TextboxFormComponent)
                        {
                            Menu.GiveFocus(imc);
                            return;
                        }
                    }
                    Menu.GiveFocus(first);
                    return;
                case 13:
                    if (EnterPressed != null)
                        Value = EnterPressed(this, Menu, Value);
                    else
                        Menu.ResetFocus();
                    return;
            }
        }
        public void RecieveSpecialInput(Keys k)
        {

        }
    }
}
