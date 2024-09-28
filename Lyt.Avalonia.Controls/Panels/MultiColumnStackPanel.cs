namespace Lyt.Avalonia.Controls.Panels;

/// <summary> A panel which lays out its children vertically, using a multi-column layout. </summary>
/// <remarks> The panel is always Vertical. </remarks>
public class MultiColumnStackPanel : Panel 
{
    /// <summary> Initializes static members of the <see cref="MultiColumnStackPanel"/> class. </summary>
    static MultiColumnStackPanel()
    {
        AffectsMeasure<MultiColumnStackPanel>(SpacingProperty);
        AffectsMeasure<MultiColumnStackPanel>(ColumnMaxWidthProperty);
    }

    /// <summary> Defines the <see cref="Spacing"/> property. </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<MultiColumnStackPanel, double>(nameof(Spacing));

    /// <summary> Gets or sets the size of the spacing to place between child controls. </summary>
    public double Spacing
    {
        get => this.GetValue(SpacingProperty);
        set => this.SetValue(SpacingProperty, value);
    }

    /// <summary> Defines the <see cref="ColumnMaxWidth"/> property. </summary>
    public static readonly StyledProperty<double> ColumnMaxWidthProperty =
        AvaloniaProperty.Register<MultiColumnStackPanel, double>(nameof(ColumnMaxWidth));

    /// <summary> Gets or sets the maximum width of the panel column. </summary>
    public double ColumnMaxWidth
    {
        get => this.GetValue(ColumnMaxWidthProperty);
        set => this.SetValue(ColumnMaxWidthProperty, value);
    }

    /// <summary>
    /// General StackPanel layout behavior is to grow unbounded in the vertical direction (Size To Content).
    /// Children in this dimension are encouraged to be as large as they like.  In the other dimension,
    /// StackPanel will assume the maximum size of its children.
    /// </summary>
    /// <param name="availableSize">Constraint</param>
    /// <returns>Desired size</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        var stackDesiredSize = new Size();
        var children = this.Children;
        Size layoutSlotSize = availableSize;
        double spacing = this.Spacing;
        bool hasOneOrMoreVisibleChild = false;

        int columnCount = (int ) ( availableSize.Width / ( this.ColumnMaxWidth + this.Spacing) ) ;
        if (columnCount == 0)
        {
            columnCount = 1; 
        }

        // Initialize child sizing and iterator data
        // Allow children as much size as they want along the stack.
        layoutSlotSize = layoutSlotSize.WithHeight(double.PositiveInfinity);

        //  Iterate through children.
        if (columnCount == 1)
        {
            for (int i = 0, count = children.Count; i < count; ++i)
            {
                // Get next child.
                var child = children[i];
                bool isVisible = child.IsVisible;
                if (isVisible)
                {
                    hasOneOrMoreVisibleChild = true;
                }

                // Measure the child.
                child.Measure(layoutSlotSize);
                Size childDesiredSize = child.DesiredSize;
                if (columnCount == 1)
                {
                    stackDesiredSize = stackDesiredSize.WithWidth(Math.Max(stackDesiredSize.Width, childDesiredSize.Width));
                    stackDesiredSize = stackDesiredSize.WithHeight(stackDesiredSize.Height + (isVisible ? spacing : 0) + childDesiredSize.Height);
                }
            }
        }
        else if (columnCount > 1)
        {
            double availableWidth = availableSize.Width / 2.0 - spacing;
            layoutSlotSize = availableSize.WithWidth(availableWidth);
            int i = 0;
            while( i < children.Count)
            {
                var left = this.GetFirstVisibleChild(i, out int nextSameRow);
                var right = this.GetFirstVisibleChild(nextSameRow, out int nextNextRow);
                i = nextNextRow;

                if ( left is null )
                {
                    // all done 
                    break; 
                }

                hasOneOrMoreVisibleChild = true;

                // Measure the left child.
                left.Measure(layoutSlotSize);

                if (right is null )
                {
                    Size childDesiredSize = left.DesiredSize;
                    stackDesiredSize = stackDesiredSize.WithWidth(Math.Max(stackDesiredSize.Width, childDesiredSize.Width));
                    stackDesiredSize = stackDesiredSize.WithHeight(stackDesiredSize.Height + spacing + childDesiredSize.Height);
                    break; 
                }
                else
                {
                    // Measure the right child.
                    right.Measure(layoutSlotSize);

                    double leftWidth = left.DesiredSize.Width;
                    double rightWidth = right.DesiredSize.Width;
                    double desiredWidth = leftWidth + rightWidth + spacing;
                    double leftHeight = left.DesiredSize.Height;
                    double rightHeight = right.DesiredSize.Height;
                    double desiredHeight = Math.Max(leftHeight, rightHeight) + spacing;
                    stackDesiredSize = stackDesiredSize.WithWidth(Math.Max(stackDesiredSize.Width, desiredWidth));
                    stackDesiredSize = stackDesiredSize.WithHeight(stackDesiredSize.Height + desiredHeight);
                }
            }
        }

        stackDesiredSize = stackDesiredSize.WithHeight(stackDesiredSize.Height - (hasOneOrMoreVisibleChild ? spacing : 0));
        return stackDesiredSize;
    }

    /// <summary> Content arrangement. </summary>
    /// <param name="finalSize">Arrange size</param>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = this.Children;
        double spacing = this.Spacing;
        double previousChildHeight = 0.0;
        int columnCount = (int)(finalSize.Width / (this.ColumnMaxWidth + this.Spacing));
        if (columnCount == 0)
        {
            columnCount = 1;
        }

        // Arrange and Position Children.
        if (columnCount == 1)
        {
            var rectangleChild = new Rect(finalSize);
            for (int i = 0, count = children.Count; i < count; ++i)
            {
                var child = children[i];

                if (!child.IsVisible)
                {
                    continue;
                }

                rectangleChild = rectangleChild.WithY(rectangleChild.Y + previousChildHeight);
                previousChildHeight = child.DesiredSize.Height;
                rectangleChild = rectangleChild.WithHeight(previousChildHeight);
                rectangleChild = rectangleChild.WithWidth(Math.Max(finalSize.Width, child.DesiredSize.Width));
                previousChildHeight += spacing;

                child.Arrange(rectangleChild);
            }
        }
        else if (columnCount > 1)
        {
            // Regardless of the actual possible count of columns, we are performing layout on two columns,
            // named left and right.
            var rectangleLeft = new Rect(finalSize);
            var rectangleRight = new Rect(finalSize);
            double availableWidth = finalSize.Width / 2.0 - spacing;
            int i = 0;
            while (i < children.Count)
            {
                var left = this.GetFirstVisibleChild(i, out int nextSameRow);
                var right = this.GetFirstVisibleChild(nextSameRow, out int nextNextRow);
                i = nextNextRow;

                if (left is null)
                {
                    // all done 
                    break;
                }

                if (right is null)
                {
                    // Last one on the left 
                    rectangleLeft = rectangleLeft.WithY(rectangleLeft.Y + previousChildHeight);
                    previousChildHeight = left.DesiredSize.Height;
                    rectangleLeft = rectangleLeft.WithHeight(previousChildHeight);
                    rectangleLeft = rectangleLeft.WithWidth(Math.Max(availableWidth, left.DesiredSize.Width));

                    left.Arrange(rectangleLeft);

                    // all done 
                    break;
                }
                else
                {
                    // Row with both left and right 
                    rectangleLeft = rectangleLeft.WithY(rectangleLeft.Y + previousChildHeight);
                    rectangleRight = rectangleRight.WithY(rectangleRight.Y + previousChildHeight);
                    rectangleRight = rectangleRight.WithX(finalSize.Width / 2.0 + spacing);

                    previousChildHeight = Math.Max(left.DesiredSize.Height, right.DesiredSize.Height);
                    rectangleLeft = rectangleLeft.WithHeight(previousChildHeight);
                    rectangleRight = rectangleRight.WithHeight(previousChildHeight);
                    rectangleLeft = rectangleLeft.WithWidth(Math.Max(availableWidth, left.DesiredSize.Width));
                    rectangleRight = rectangleRight.WithWidth(Math.Max(availableWidth, right.DesiredSize.Width));

                    previousChildHeight += spacing;

                    left.Arrange(rectangleLeft);
                    right.Arrange(rectangleRight);
                }
            }
        }

        return finalSize;
    }

    Control? GetFirstVisibleChild(int startIndex, out int nextIndex)
    {
        var children = this.Children;
        while (startIndex < children.Count)
        {
            // Get next visible child starting at startIndex
            var child = children[startIndex];
            if (child.IsVisible)
            {
                nextIndex = startIndex + 1;
                return child;
            }
            else
            {
                ++startIndex;
            }
        }

        nextIndex = -1;
        return null;
    }
}