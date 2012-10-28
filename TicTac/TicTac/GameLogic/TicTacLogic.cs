using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GameLogic
{
    class TicTacLogic
    {
        public Texture2D x;
        public Texture2D o;

        public Texture2D Result;
        public Texture2D ResultX;
        public Texture2D ResultO;
        public Texture2D ResultNo;
        
        class Field
        {
            public enum FieldState
            {
                blank,
                x,
                zero
            }
            public FieldState state;
            public Rectangle touchRect;
            public Vector2 position;
            public Field() { state = FieldState.blank;}
        }

        Field[] fields;

        Field.FieldState currentFieldState;
        public bool finished;

        public void Initialize()
        {
            fields = new Field[9];
            //attach rects and positions
            fields[0] = new Field {touchRect = new Rectangle(10, 155, 135, 140), position = new Vector2(10, 160)};
            fields[1] = new Field {touchRect = new Rectangle(170, 155, 135, 140),position = new Vector2(170, 160)};
            fields[2] = new Field {touchRect = new Rectangle(325, 155, 145, 140),position = new Vector2(325, 160)};
            fields[3] = new Field {touchRect = new Rectangle(10, 320, 135, 140),position = new Vector2(10, 322)};
            fields[4] = new Field {touchRect = new Rectangle(170, 320, 135, 140),position = new Vector2(170, 322)};
            fields[5] = new Field {touchRect = new Rectangle(325, 320, 145, 140),position = new Vector2(325, 322)};
            fields[6] = new Field {touchRect = new Rectangle(10, 480, 135, 140),position = new Vector2(10, 480)};
            fields[7] = new Field {touchRect = new Rectangle(170, 480, 135, 140),position = new Vector2(170, 480)};
            fields[8] = new Field { touchRect = new Rectangle(325, 480, 145, 140), position = new Vector2(325, 480) };

            Begin();
        }

        public void Begin()
        {
            for (int i = 1; i <= 9; i++) { fields[i-1].state = Field.FieldState.blank; }
            currentFieldState = Field.FieldState.x;
            finished = false;
            Result = ResultNo;
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 1; i <= 9; i++)
            {
                switch (fields[i-1].state)
                {
                    case Field.FieldState.x:
                        spriteBatch.Draw(x, fields[i-1].position, new Color(192, 192, 192));
                        break;
                    case Field.FieldState.zero:
                        spriteBatch.Draw(o, fields[i-1].position, new Color(192, 192, 192));
                        break;
                }
            }
        }

        public void ClickRect(Rectangle rect)
        {
            if (!finished)
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (fields[i-1].state == Field.FieldState.blank)
                    {
                        if (fields[i-1].touchRect.Intersects(rect))
                        {
                            fields[i-1].state = currentFieldState;
                            currentFieldState = currentFieldState == Field.FieldState.x ? Field.FieldState.zero : Field.FieldState.x;
                            checkLines();
                            break;
                        }
                    }
                }
            }
        }

        private void checkLines()
        {
            // 123,456,789,147,258,369,159,357
            if ((fields[0].state == fields[1].state) && (fields[0].state == fields[2].state) && (fields[01].state == Field.FieldState.x) ||
                (fields[3].state == fields[4].state) && (fields[3].state == fields[5].state) && (fields[3].state == Field.FieldState.x) ||
                (fields[6].state == fields[7].state) && (fields[6].state == fields[8].state) && (fields[6].state == Field.FieldState.x) ||
                (fields[0].state == fields[3].state) && (fields[0].state == fields[6].state) && (fields[0].state == Field.FieldState.x) ||
                (fields[1].state == fields[4].state) && (fields[1].state == fields[7].state) && (fields[1].state == Field.FieldState.x) ||
                (fields[2].state == fields[5].state) && (fields[2].state == fields[8].state) && (fields[2].state == Field.FieldState.x) ||
                (fields[0].state == fields[4].state) && (fields[0].state == fields[8].state) && (fields[0].state == Field.FieldState.x) ||
                (fields[2].state == fields[4].state) && (fields[2].state == fields[6].state) && (fields[2].state == Field.FieldState.x)
                ) { currentFieldState = Field.FieldState.x; finished = true; Result = ResultX; };
            if ((fields[0].state == fields[1].state) && (fields[0].state == fields[2].state) && (fields[01].state == Field.FieldState.zero) ||
                (fields[3].state == fields[4].state) && (fields[3].state == fields[5].state) && (fields[3].state == Field.FieldState.zero) ||
                (fields[6].state == fields[7].state) && (fields[6].state == fields[8].state) && (fields[6].state == Field.FieldState.zero) ||
                (fields[0].state == fields[3].state) && (fields[0].state == fields[6].state) && (fields[0].state == Field.FieldState.zero) ||
                (fields[1].state == fields[4].state) && (fields[1].state == fields[7].state) && (fields[1].state == Field.FieldState.zero) ||
                (fields[2].state == fields[5].state) && (fields[2].state == fields[8].state) && (fields[2].state == Field.FieldState.zero) ||
                (fields[0].state == fields[4].state) && (fields[0].state == fields[8].state) && (fields[0].state == Field.FieldState.zero) ||
                (fields[2].state == fields[4].state) && (fields[2].state == fields[6].state) && (fields[2].state == Field.FieldState.zero)
                ) { currentFieldState = Field.FieldState.zero; finished = true; Result = ResultO; };
            bool nowin = true;
            for (int i = 1; i <= 9; i++)
            {
                if (fields[i - 1].state == Field.FieldState.blank) { nowin = false; Debug.WriteLine(i); break; }
            }
            if (nowin) { finished = true; Result = ResultNo; } 
        }
    }
}
