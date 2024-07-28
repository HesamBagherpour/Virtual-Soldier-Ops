using System.Collections.Generic;
using ArioSoren.InjectKit;
using ArioSoren.UIKit.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArioSoren.UIKit.Controllers
{
    public class UIController : MonoModule
    {
        private PageBaseUI current;
        public List<PageBaseUI> pages;
        private PageBaseUI previous;
        public List<PageBaseUI> openPages = new();
        private PageBaseUI mainmune; 
    
        private async void Init(PageType type)
        {
            current = pages.Find(t => t.Type == type);
            openPages.Add(current);

            pages.ForEach(t => t.Init());
            await UniTask.Delay(100);
            current.Show();
        }
        
        private void OnEnable()
        {
            if (current!=null)
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    pages[i].OnOpenPage += OpenPage;
                    pages[i].OnClosePage += ClosePage;
                }
               
            }

           
        }
        private void OnDisable()
        {
            if (current != null)
            {
                current.OnOpenPage -= OpenPage;
                current.OnClosePage -= ClosePage;
            }
        }
        
        
        
        public void OpenPage(PageType type)
        {
            Debug.Log("this");
            var page = pages.Find(t => t.Type == type);
            if (page == null)
                return;
            if (current == null)
                return;
            current.Hide();
            previous = current;
            previous.Hide();
            current = page;
            openPages.Add(current);

            current.Show();
          
            Debug.Log("open");
        }

        public void ClosePage()
        {
            Debug.Log("// " + previous);
            current.Hide();
            print("CURRENT BEFORE " + current.rootCanvas.transform.name);

            if (openPages.Count > 0)
            {
                openPages.Remove(openPages[^1]);
                previous = openPages[^1];
            }

            current = previous;

            print("CURRENT AFTER " + current.rootCanvas.transform.name);
            current.Show();
        }

        public void CloseAllPages()
        {
            current.root.SetActive(false);
            current = null;
            openPages.Clear();
        }
    }
}
