namespace SolidSimplification.HelperMethods
{
    public class Node
    {
        public int id;
        public double x;
        public double y;
        public double cos; // Used for middlepoint calculations
        public Node(double x, double y, int id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }
        public Node(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}