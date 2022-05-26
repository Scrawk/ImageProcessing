
namespace ImageProcessing.Images
{

      public partial class GreyScaleImage2D
      {
	
	      private int[] h = null;
	private double[] M0 = null;		// table of background means
	private double[] M1 = null;		// table of foreground means
	private int N = 0;	
	      
	public int getThreshold(Histogram hist) 
        {
		h = hist;
		int K = h.length;
		makeMeanTables(h);

		double sigma2Bmax = 0;
		int qMax = -1;
		int n0 = 0;
		
		// examine all possible threshold values q:
		for (int q = 0; q <= K-2; q++)
		{
			n0 = n0 +  h[q]; 
			int n1 = N - n0;
			
			if (n0 > 0 && n1 > 0) 
			{
				double meanDiff = M0[q] - M1[q];
				double sigma2B =  meanDiff * meanDiff * n0 * n1; 
				// (1/N^2) has been omitted
				
				if (sigma2B > sigma2Bmax) 
				{
					sigma2Bmax = sigma2B;
					qMax = q;
				}
			}
		}
		
		return qMax;
	}
	
	private void makeMeanTables(Histogram h) 
	{
		int K = h.length;
		this.M0 = new double[K];
		this.M1 = new double[K];
		int n0 = 0;
		long s0 = 0;
		
		for (int q = 0; q < K; q++)
		{
			n0 = n0 + h[q];
			s0 = s0 + q * h[q];
			M0[q] = (n0 > 0) ? ((double) s0)/n0 : -1;
		}
		
		this.N = n0;
		
		int n1 = 0;
		long s1 = 0;
		M1[K-1] = 0;
		
		for (int q = h.length-2; q >= 0; q--)
		{
			n1 = n1 + h[q+1];
			s1 = s1 + (q+1) * h[q+1];
			M1[q] = (n1 > 0) ? ((double) s1)/n1 : -1;
		}
		

	}
      }
}
