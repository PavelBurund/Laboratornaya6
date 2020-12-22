using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.IO;
using System;

public enum TypeShape
{
    Rectangle, Square, Triangle, Circle
}

public interface Shape
{
    double calcArea();
    double calcPerimeter();
    double[] getParameters();
    TypeShape getType();
};

public class Triangle : Shape
{
    private double side1 = 0;
    private double side2 = 0;
    private double side3 = 0;
    public Triangle(double side1, double side2, double side3)
    {
        this.side1 = side1;
        this.side2 = side2;
        this.side3 = side3;
        if (!(side1 + side2 > side3 || side2 + side3 > side1 || side1 + side3 > side2)) throw new Exception("Вы ввели несуществущий треугольник");
        if (side1 <= 0 || side2 <= 0 || side3 <= 0) throw new Exception("Некорректное значение сторон");
    }

    public double calcArea()
    {
        double Perimetr, HalfP;
        Perimetr = side1 + side2 + side3;
        HalfP = Perimetr / 2;
        return Math.Sqrt(HalfP * (HalfP - side1) * (HalfP - side2) * (HalfP - side3));
    }
    public double calcPerimeter() { return side1 + side2 + side3; }
    public double[] getParameters() { return new double[] { side1, side2, side3 }; }
    public TypeShape getType() => TypeShape.Triangle;
};

public class Square : Shape
{
    private double side = 0;
    public Square(double side)
    {
        this.side = side;
        if (side <= 0) throw new Exception("Некорректное значение стороны");
    }

    public double calcArea() { return Math.Pow(side, 2); }
    public double calcPerimeter() { return side * 4; }
    public double[] getParameters() { return new double[] { side }; }
    public TypeShape getType() => TypeShape.Square;
};

public class Rectangle : Shape
{
    private double width = 0;
    private double height = 0;
    public Rectangle(double width, double height)
    {
        this.width = width;
        this.height = height;
        if (width <= 0 || height <= 0) throw new Exception("Некорректное значение сторон");
    }

    public double calcArea() { return width * height; }
    public double calcPerimeter() { return width * 2 + height * 2; }
    public double[] getParameters() { return new double[] { width, height }; }
    public TypeShape getType() => TypeShape.Rectangle;
}

public class Circle : Shape
{
    private double radius = 0;
    public Circle(double radius)
    {
        this.radius = radius;
        if (radius <= 0) throw new Exception("Некорректное значение радиуса");
    }

    public double calcArea() { return Math.PI * radius * radius; }
    public double calcPerimeter() { return Math.PI * 2 * radius; }
    public double[] getParameters() { return new double[] { radius }; }
    public TypeShape getType() => TypeShape.Circle;
};

public class Main : MonoBehaviour
{
    [SerializeField] private InputField[] _TriangleFields;
    [SerializeField] private InputField[] _SquareFields;
    [SerializeField] private InputField[] _RectangleFields;
    [SerializeField] private InputField[] _CircleFields;

    [SerializeField] private Transform _PanelElements;
    [SerializeField] private GameObject _PrefabElement;

    private const string fileName = "Bobrov&Burundukov.json";
    [SerializeField] private List<Shape> _Shapes;

    private int _currentIndex;

    private void Awake()
    {

        _Shapes = new List<Shape>();

        if (File.Exists(fileName))
        {
            List<string> array = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(fileName));
            foreach (var element in array)
            {
                TypeShape figure = (TypeShape)(int.Parse(JsonConvert.DeserializeObject<string[]>(element)[0]));
                switch (figure)
                {
                    case TypeShape.Circle:
                        {
                            double[] parameters = JsonConvert.DeserializeObject<double[]>(JsonConvert.DeserializeObject<string[]>(element)[1]);
                            _Shapes.Add(new Circle(parameters[0]));
                            break;
                        }
                    case TypeShape.Rectangle:
                        {
                            double[] parameters = JsonConvert.DeserializeObject<double[]>(JsonConvert.DeserializeObject<string[]>(element)[1]);
                            _Shapes.Add(new Rectangle(parameters[0], parameters[1]));
                            break;
                        }
                    case TypeShape.Square:
                        {
                            double[] parameters = JsonConvert.DeserializeObject<double[]>(JsonConvert.DeserializeObject<string[]>(element)[1]);
                            _Shapes.Add(new Square(parameters[0]));
                            break;
                        }
                    case TypeShape.Triangle:
                        {
                            double[] parameters = JsonConvert.DeserializeObject<double[]>(JsonConvert.DeserializeObject<string[]>(element)[1]);
                            _Shapes.Add(new Triangle(parameters[0], parameters[1], parameters[2]));
                            break;
                        }
                }
            }
        }
        else
        {
            string json = "[]";
            File.WriteAllText(fileName, json);
        }

    }

    private void Start()
    {
        for (int i = 0; i < _Shapes.Count; i++)
        {
            CreateShape(_Shapes[i].getType(), i);
        }
    }

    private void OnApplicationQuit()
    {
        List<string> array = new List<string>();
        foreach (var element in _Shapes)
        {
            array.Add(JsonConvert.SerializeObject(new string[] { ((int)element.getType()).ToString(), JsonConvert.SerializeObject(element.getParameters()) }));
        }

        string json = JsonConvert.SerializeObject(array);
        File.WriteAllText(fileName, json);
    }

    public void SetElement(Transform transform) => _currentIndex = int.Parse(transform.name);

    public void Remove()
    {
        _Shapes.RemoveAt(_currentIndex);
        Destroy(GameObject.Find(_currentIndex.ToString()));
    }

    public void MoveUp()
    {
        var shape = _Shapes[_currentIndex];
        _Shapes[_currentIndex] = _Shapes[_currentIndex - 1];
        _Shapes[_currentIndex - 1] = shape;

        Transform element = _PanelElements.GetChild(_currentIndex + 1);
        element.SetSiblingIndex(element.GetSiblingIndex() - 1);
        element.name = (int.Parse(element.name) - 1).ToString();

        Transform movedElement = _PanelElements.GetChild(_currentIndex + 1);
        movedElement.name = (int.Parse(movedElement.name) + 1).ToString();
        _currentIndex--;
    }

    public void MoveDown()
    {
        var shape = _Shapes[_currentIndex];
        _Shapes[_currentIndex] = _Shapes[_currentIndex + 1];
        _Shapes[_currentIndex + 1] = shape;

        Transform element = _PanelElements.GetChild(_currentIndex + 1);
        element.SetSiblingIndex(element.GetSiblingIndex() + 1);
        element.name = (int.Parse(element.name) + 1).ToString();

        Transform movedElement = _PanelElements.GetChild(_currentIndex + 1);
        movedElement.name = (int.Parse(movedElement.name) - 1).ToString();
        _currentIndex++;
    }

    public void CreateTriangle()
    {
        _Shapes.Add(new Triangle(double.Parse(_TriangleFields[0].text), double.Parse(_TriangleFields[1].text), double.Parse(_TriangleFields[2].text)));
        CreateShape(TypeShape.Triangle, _Shapes.Count - 1);
    }

    public void CreateSquare()
    {
        _Shapes.Add(new Square(double.Parse(_SquareFields[0].text)));
        CreateShape(TypeShape.Square, _Shapes.Count - 1);
    }

    public void CreateRectangle()
    {
        _Shapes.Add(new Rectangle(int.Parse(_RectangleFields[0].text), int.Parse(_RectangleFields[1].text)));
        CreateShape(TypeShape.Rectangle, _Shapes.Count - 1);
    }

    public void CreateCircle()
    {
        _Shapes.Add(new Circle(double.Parse(_CircleFields[0].text)));
        CreateShape(TypeShape.Circle, _Shapes.Count - 1);
    }

    private void CreateShape(TypeShape type, int index)
    {
        switch (type)
        {
            case TypeShape.Circle:
                {
                    var gameObject = Instantiate(_PrefabElement, _PanelElements);
                    gameObject.name = index.ToString();
                    gameObject.GetComponentInChildren<Text>().text = $"Circle | {_Shapes[index].getParameters()[0]}";
                    gameObject.SetActive(true);
                }
                break;
            case TypeShape.Rectangle:
                {
                    var gameObject = Instantiate(_PrefabElement, _PanelElements);
                    gameObject.name = index.ToString();
                    gameObject.GetComponentInChildren<Text>().text = $"Rectangle |  {_Shapes[index].getParameters()[0]},{_Shapes[index].getParameters()[1]}";
                    gameObject.SetActive(true);
                }
                break;
            case TypeShape.Square:
                {
                    var gameObject = Instantiate(_PrefabElement, _PanelElements);
                    gameObject.name = index.ToString();
                    gameObject.GetComponentInChildren<Text>().text = $"Square | {_Shapes[index].getParameters()[0]}";
                    gameObject.SetActive(true);
                }
                break;
            case TypeShape.Triangle:
                {
                    var gameObject = Instantiate(_PrefabElement, _PanelElements);
                    gameObject.name = index.ToString();
                    gameObject.GetComponentInChildren<Text>().text = $"Triangle | {_Shapes[index].getParameters()[0]},{_Shapes[index].getParameters()[1]},{_Shapes[index].getParameters()[2]}";
                    gameObject.SetActive(true);
                }
                break;
        }
    }
}
