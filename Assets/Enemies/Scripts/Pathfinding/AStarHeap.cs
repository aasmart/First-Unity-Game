using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.Scripts.Pathfinding
{
    public class AStarHeap
    {
        private Node[] _items;
        private int _itemCount;

        public AStarHeap(int maxHeapSize) 
        {
            _items = new Node[maxHeapSize];
        }

        public void Add(Node item, GameObject objectKey)
        {
            item.GetData(objectKey).HeapIndex = _itemCount;
            _items[_itemCount] = item;
            SortUp(item, objectKey);
            _itemCount++;
        }

        public Node RemoveFirst(GameObject objectKey)
        {
            var firstItem = _items[0];
            _itemCount--;
            _items[0] = _items[_itemCount];
            _items[0].GetData(objectKey).HeapIndex = 0;
            SortDown(_items[0], objectKey);

            return firstItem;
        }
        
        public bool Contains(Node item, GameObject objectKey) {
            return Equals(_items[item.GetData(objectKey).HeapIndex], item);
        }

        public void UpdateItem(Node item, GameObject objectKey) {
            SortUp(item, objectKey);
        }

        public int GetItemCount() {
            return _itemCount;
        }
        
        public void SortUp(Node item, GameObject objectKey) {
            var parentIndex = (item.GetData(objectKey).HeapIndex - 1) / 2;

            while(true) {
                var parentItem = _items[parentIndex];
                if(item.CompareTo(parentItem, objectKey) > 0)
                    Swap(item, parentItem, objectKey);
                else
                    break;

                parentIndex = (item.GetData(objectKey).HeapIndex - 1) / 2;
            }
        }

        public void SortDown(Node item, GameObject objectKey) {
            while (true) {
                var leftChildIndex = item.GetData(objectKey).HeapIndex * 2 + 1;
                var rightChildIndex = item.GetData(objectKey).HeapIndex * 2 + 2;

                if(leftChildIndex < _itemCount) {
                    var swapIndex = leftChildIndex;
                    if(rightChildIndex < _itemCount) {
                        if(_items[leftChildIndex].CompareTo(_items[rightChildIndex], objectKey) < 0)
                            swapIndex = rightChildIndex;
                    }

                    if(item.CompareTo(_items[swapIndex], objectKey) < 0)
                        Swap(item, _items[swapIndex], objectKey);
                    else
                        return;
                } else
                    return;
            }
        }

        private void Swap(Node itemA, Node itemB, GameObject objectKey) {
            _items[itemA.GetData(objectKey).HeapIndex] = itemB;
            _items[itemB.GetData(objectKey).HeapIndex] = itemA;
            
            (itemA.GetData(objectKey).HeapIndex, itemB.GetData(objectKey).HeapIndex) = (itemB.GetData(objectKey).HeapIndex, itemA.GetData(objectKey).HeapIndex);
        }
    }

    public interface IHeapItem
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
    
    /* public class Heap<T> where T : IHeapItem<T>
    {
        private T[] _items;
        private int _itemCount;

        public Heap(int maxHeapSize) 
        {
            _items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = _itemCount;
            _items[_itemCount] = item;
            SortUp(item);
            _itemCount++;
        }

        public T RemoveFirst()
        {
            var firstItem = _items[0];
            _itemCount--;
            _items[0] = _items[_itemCount];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);

            return firstItem;
        }
        
        public bool Contains(T item) {
            return Equals(_items[item.HeapIndex], item);
        }

        public void UpdateItem(T item) {
            SortUp(item);
        }

        public int GetItemCount() {
            return _itemCount;
        }
        
        public void SortUp(T item) {
            var parentIndex = (item.HeapIndex - 1) / 2;

            while(true) {
                var parentItem = _items[parentIndex];
                if(item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        public void SortDown(T item) {
            while (true) {
                var leftChildIndex = item.HeapIndex * 2 + 1;
                var rightChildIndex = item.HeapIndex * 2 + 2;

                if(leftChildIndex < _itemCount) {
                    var swapIndex = leftChildIndex;
                    if(rightChildIndex < _itemCount) {
                        if(_items[leftChildIndex].CompareTo(_items[rightChildIndex]) < 0)
                            swapIndex = rightChildIndex;
                    }

                    if(item.CompareTo(_items[swapIndex]) < 0)
                        Swap(item, _items[swapIndex]);
                    else
                        return;
                } else
                    return;
            }
        }

        private void Swap(T itemA, T itemB) {
            _items[itemA.HeapIndex] = itemB;
            _items[itemB.HeapIndex] = itemA;
            
            (itemA.HeapIndex, itemB.HeapIndex) = (itemB.HeapIndex, itemA.HeapIndex);
        }
    }

    public interface IHeapItem<in T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }*/
}
