using Common.Core.Colors;
using ImageProcessing.Images;
using System.Collections.Generic;

namespace ImageProcessing.Synthesis
{
    public class Exemplar
    {

        public Exemplar(ColorImage2D image)
        {
            Image = image;
        }

        public Exemplar(ColorImage2D image, Exemplar original)
        {
            Image = image;
            Original = original;
        }

        public int Width => Image.Width;

        public int Height => Image.Height;

        public int Used { get; private set; }

        public bool IsVariant => Original != null;

        public ColorRGB this[int x, int y]
        {
            get { return Image[x, y]; }
        }

        public ColorImage2D Image { get; private set; }

        private Exemplar Original {  get; set; }

        public void IncrementUsed()
        {
            Used++;
        }

        public List<Exemplar> CreateVariants()
        {
            var variants = new List<Exemplar>();

            variants.Add(new Exemplar(ColorImage2D.Rotate90(Image), this));
            variants.Add(new Exemplar(ColorImage2D.Rotate180(Image), this));
            variants.Add(new Exemplar(ColorImage2D.Rotate270(Image), this));

            return variants;
        }

    }
}
