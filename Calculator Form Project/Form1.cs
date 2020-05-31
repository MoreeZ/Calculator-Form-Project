using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Calculator_Form_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void CalculateEquation()
        {
            this.label1.Text = ParseOperation();
        }

        private string ParseOperation()
        {
            try
            {
                var userInput = this.textBox1.Text;
                userInput = userInput.Replace(" ", "");

                var operation = new Operation();
                var leftSide = true;

                for (int i = 0; i < userInput.Length; i++)
                {
                    //if character is a number or a dot
                    if ("1234567890.".Any(c => userInput[i] == c))
                    {
                        if (leftSide)
                            operation.LeftSide = AddNumberPart(operation.LeftSide, userInput[i]);
                        else
                            operation.RightSide = AddNumberPart(operation.RightSide, userInput[i]);
                    }
                    // If character is an operator
                    else if ("+-*/".Any(c => userInput[i] == c))
                    {

                        if (!leftSide)
                        {
                            // Get the operator type
                            var operatorType = GetOperationType(userInput[i]);

                            // Check if we actually have a right side number
                            if (operation.RightSide.Length == 0)
                            {
                                // Check the operator is not a minus (as they could be creating a negative number)
                                if (operatorType != OperationType.Minus)
                                    throw new InvalidOperationException($"Operator (+ * / or more than one -) specified without an right side number");

                                // If we got here, the operator type is a minus, and there is no left number currently, so add the minus to the number
                                operation.RightSide += userInput[i];
                            }
                            else
                            {
                                // Calculate previous equation and set to the left side
                                operation.LeftSide = CalculateOperation(operation);

                                // Set new operator
                                operation.OperationType = operatorType;

                                // Clear the previous right number
                                operation.RightSide = string.Empty;
                            }
                        }
                        else
                        {
                            // Get the operator type
                            var operatorType = GetOperationType(userInput[i]);

                            // Check if we actually have a left side number
                            if (operation.LeftSide.Length == 0)
                            {
                                // Check the operator is not a minus (as they could be creating a negative number)
                                if (operatorType != OperationType.Minus)
                                    throw new InvalidOperationException($"Operator (+ * / or more than one -) specified without an left side number");

                                // If we got here, the operator type is a minus, and there is no left number currently, so add the minus to the number
                                operation.LeftSide += userInput[i];
                            }
                            else
                            {
                                // If we get here, we have a left number and now an operator, so we want to move to the right side

                                // Set the operation type
                                operation.OperationType = operatorType;

                                // Move to the right side
                                leftSide = false;
                            }
                        }
                    }
                    
                }

                return CalculateOperation(operation);
            }
            catch(Exception ex)
            {
                return $"Invalid equation. {ex.Message}";
            }

        }

        private string CalculateOperation(Operation operation)
        {
            // Store number values of the string representations
            decimal left = 0;
            decimal right = 0;

            // Check if we have a valid left side number
            if (string.IsNullOrEmpty(operation.LeftSide) || !decimal.TryParse(operation.LeftSide, out left))
                throw new InvalidOperationException($"Left side of the operation was not a number. {operation.LeftSide}");

            // Check if we have a valid right side number
            if (string.IsNullOrEmpty(operation.RightSide) || !decimal.TryParse(operation.RightSide, out right))
            {
                MessageBox.Show($"left: {left}, right: {right}, operation type: {operation.OperationType}");
                throw new InvalidOperationException($"Right side of the operation was not a number. {operation.RightSide}");
            }

            try
            {
                switch(operation.OperationType)
                {
                    case OperationType.Add:
                        return (left + right).ToString();
                    case OperationType.Minus:
                        return (left - right).ToString();
                    case OperationType.Divide:
                        return (left / right).ToString();
                    case OperationType.Multiply:
                        return (left * right).ToString();
                    default:
                        throw new InvalidOperationException($"Unknown operator type when calculating operation. { operation.OperationType }");
                }
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Unknown operation type when calculating operation {operation.LeftSide} {operation.OperationType} {operation.RightSide}. {ex.Message}");
            }

            
        }

        private OperationType GetOperationType(char character)
        {
            switch (character)
            {
                case '+':
                    return OperationType.Add;
                case '-':
                    return OperationType.Minus;
                case '/':
                    return OperationType.Divide;
                case '*':
                    return OperationType.Multiply;
                default:
                    throw new InvalidOperationException($"Unknown operator type { character }");
            }
        }

        private string AddNumberPart(string currentNumber, char newCharacter)
        {
            // Check if there is already a . in the number
            if (newCharacter == '.' && currentNumber.Contains('.'))
                throw new InvalidOperationException($"Number {currentNumber} already contains a . and another cannot be added");

            return currentNumber + newCharacter;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CalculateEquation();
        }

    }
}
