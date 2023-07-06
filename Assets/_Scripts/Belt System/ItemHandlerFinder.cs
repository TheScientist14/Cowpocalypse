using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandlerFinder : MonoBehaviour, IEnumerable<IItemHandler>
{
    [Serializable]
    private class ItemHandlerSearch
    {
        public IItemHandler FoundItemHandler = null;
        public bool IsSearching = false;
        public Vector4 LocalDeltaToTarget;

        public ItemHandlerSearch(Vector3 iLocalToTarget)
        {
            LocalDeltaToTarget = iLocalToTarget;
            LocalDeltaToTarget.w = 1;
        }
    }

    private List<ItemHandlerSearch> m_ItemHandlerSearches = new List<ItemHandlerSearch>();

    public struct ItemHandlerSearchIterator : IEnumerator<IItemHandler>
    {
        private IEnumerator<ItemHandlerSearch> m_SearchIt;

        public ItemHandlerSearchIterator(ItemHandlerFinder iFinder)
        {
            m_SearchIt = iFinder.m_ItemHandlerSearches.GetEnumerator();
        }

        public IItemHandler Current => m_SearchIt.Current.FoundItemHandler;

        object IEnumerator.Current => m_SearchIt.Current.FoundItemHandler;

        public void Dispose()
        {
            m_SearchIt.Dispose();
        }

        public bool MoveNext()
        {
            return m_SearchIt.MoveNext();
        }

        public void Reset()
        {
            m_SearchIt.Reset();
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ItemHandlerSearch search in m_ItemHandlerSearches)
        {
            if(search.FoundItemHandler == null && !search.IsSearching)
                StartCoroutine(_Search(search));
        }
    }

    IEnumerator _Search(ItemHandlerSearch iSearch)
    {
        iSearch.IsSearching = true;
        Vector3 target = transform.localToWorldMatrix * iSearch.LocalDeltaToTarget;

        Collider2D[] collider = new Collider2D[1];
        while(iSearch.FoundItemHandler == null)
        {
            int nbHit = Physics2D.OverlapCircleNonAlloc(target, .1f, collider);
            if(nbHit > 0)
                iSearch.FoundItemHandler = collider[0].GetComponent<IItemHandler>();

            yield return new WaitForSecondsRealtime(.1f);
        }

        iSearch.IsSearching = false;
    }

    // This function only adds a target to search for
    public void AddItemHandlerSearch(Vector3 iLocalToTarget)
    {
        ItemHandlerSearch search = new ItemHandlerSearch(iLocalToTarget);

        // avoiding doublons
        foreach(ItemHandlerSearch exisitingSearch in m_ItemHandlerSearches)
        {
            if(exisitingSearch.LocalDeltaToTarget == search.LocalDeltaToTarget)
                return;
        }

        m_ItemHandlerSearches.Add(search);
    }

    public void RefreshSearches()
    {
        StopAllCoroutines();

        foreach(ItemHandlerSearch search in m_ItemHandlerSearches)
        {
            search.FoundItemHandler = null;
            search.IsSearching = false;
        }
    }

    public IItemHandler this[int iItemHandlerIdx]
    {
        get { return m_ItemHandlerSearches[iItemHandlerIdx].FoundItemHandler; }
    }

    IEnumerator<IItemHandler> IEnumerable<IItemHandler>.GetEnumerator()
    {
        return new ItemHandlerSearchIterator(this);
    }

    public IEnumerator GetEnumerator()
    {
        return new ItemHandlerSearchIterator(this);
    }

    public int Count
    {
        get { return m_ItemHandlerSearches.Count; }
    }
}
