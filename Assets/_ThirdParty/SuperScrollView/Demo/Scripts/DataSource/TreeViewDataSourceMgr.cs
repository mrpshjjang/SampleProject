﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{    
    public class TreeViewDataSourceMgr<T> where T : ItemDataBase, new()
    {
        List<TreeViewItemData<T>> mItemDataList = new List<TreeViewItemData<T>>();
        int mTreeViewItemCount = 1000;
        int mTreeViewChildItemCountDefault = 30;
        static int[] mTreeViewChildItemCount = 
        {  
            2,3,7,2,8,4,10,5,9,30,
            2,3,7,2,8,4,10,5,9,30,
        };   

        public int TreeViewItemCount
        {
            get
            {
                return mItemDataList.Count;
            }
        }     

        public int TotalTreeViewItemAndChildCount
        {
            get
            {
                int count =  mItemDataList.Count;
                int totalCount = 0;
                for(int i = 0;i<count;++i)
                {
                    totalCount = totalCount + mItemDataList[i].ChildCount;
                }
                return totalCount;
            }
        }
        
        public TreeViewDataSourceMgr()
        {
            DoRefreshDataSource();
        }        

        public TreeViewItemData<T> GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= mItemDataList.Count)
            {
                return null;
            }
            return mItemDataList[index];
        }

        public T GetItemChildDataByIndex(int itemIndex,int childIndex)
        {
            TreeViewItemData<T> itemData = GetItemDataByIndex(itemIndex);
            if(itemData == null)
            {
                return null;
            }
            return itemData.GetItemChildDataByIndex(childIndex);
        }        

        public T AddNewItemChild(int itemIndex,int AddToBeforeChildIndex)
        {
            if (itemIndex < 0 || itemIndex >= mItemDataList.Count)
            {
                return null;
            }
            TreeViewItemData<T> itemData = mItemDataList[itemIndex];
            return itemData.AddNewItemChild(itemIndex, AddToBeforeChildIndex);            
        }


        public void AddItemChild(int itemIndex, int AddToBeforeChildIndex,T itemData)
        {
            if (itemIndex < 0 || itemIndex >= mItemDataList.Count)
            {
                return;
            }
            TreeViewItemData<T> treeViewItemData = mItemDataList[itemIndex];
            treeViewItemData.AddChildByIndex(itemIndex, AddToBeforeChildIndex,itemData);
        }

        public void RemoveItemChild(int itemIndex, int childIndex)
        {
            if (itemIndex < 0 || itemIndex >= mItemDataList.Count)
            {
                return;
            }
            TreeViewItemData<T> itemData = mItemDataList[itemIndex];
            itemData.RemoveChildByIndex(itemIndex,childIndex);
        }

        void DoRefreshDataSource()
        {
            mItemDataList.Clear();
            for (int i = 0; i < mTreeViewItemCount; ++i)
            {
                TreeViewItemData<T> itemData = new TreeViewItemData<T>();
                mItemDataList.Add(itemData);
                int count = mTreeViewChildItemCountDefault;
                if( i < mTreeViewChildItemCount.Length)
                {
                    count = mTreeViewChildItemCount[i];
                }
                itemData.RefreshItemDataList(i, count);
            }
        }  
    }
}