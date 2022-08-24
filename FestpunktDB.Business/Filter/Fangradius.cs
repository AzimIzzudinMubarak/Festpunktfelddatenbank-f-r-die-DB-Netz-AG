using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestpunktDB.Business;
using FestpunktDB.Business.Entities;

namespace FestpunktDB.Business
{
    public static class Fangradius
    {
        

        public static ExportFilterContext DbFilter = new ExportFilterContext();
        public static EntityFrameworkContext DbGlobal = new EntityFrameworkContext();
        
        /// <summary>
        /// Filter if only Fangradius PS0-PS1 is used
        /// </summary>
        /// <param name="listAv">Avani list</param>
        /// <param name="inputFang1">Textbox input</param>
        /// <param name="listPp">Pp list</param>
        public static void FangradiusPS0PS1(List<Avani> listAv, double inputFang1,List<Pp> listPp)
        {
            
            var filteredAv = DbFilter.Avani.ToList();
            var filteredPp = DbGlobal.Pp.ToList();
            
            filteredAv.RemoveAll(x => listAv.Exists(y => y.Pad == x.Pad));
            
            List<Avani> tempListAvani = new List<Avani>();
           
          
            //calcualte every distance foreach points in lists
            for (int i = 0; i <= listAv.Count - 1; i++)
            {
                for (int k = 0; k <= filteredAv.Count - 1; k++)
                {
                    var result = Math.Sqrt(Math.Pow(listAv[i].Lx - filteredAv[k].Lx, 2) + Math.Pow(listAv[i].Ly - filteredAv[k].Ly, 2));

                    //if result is lower than input text save items in list
                    if (result <= inputFang1)
                    {
                        if (filteredAv[k].Part.Equals("PS0") || filteredAv[k].Part.Equals("PS1"))
                            tempListAvani.Add(filteredAv[k]);                        
                    }
                }
            }
            //elimante duplicates from filteredAv and filteredpp
            filteredPp.RemoveAll(x => listAv.Exists(y => x.PAD == y.Pad));
            //eliminate every duplicate from result list
            IEnumerable<Avani> distinctListAvani = tempListAvani.Distinct();
            
            //Add result points to final Av list
            foreach(var item in distinctListAvani)
            {
                listAv.Add(item);

                
            }
            
            //compare finalAv to filteredPp and add missing points to dataGrid
            listPp.AddRange(filteredPp.Where(x => listAv.Exists(y => y.Pad == x.PAD)));
          
        }
    /// <summary>
    /// Filter if only Fangradius PS2-PS4 is used
    /// </summary>
    /// <param name="listAv">Avani list</param>
    /// <param name="inputFang2">Textbox input</param>
    /// <param name="listPp">Pp list</param>
    public static void FangradiusPS2PS4(List<Avani> listAv, double inputFang2, List<Pp> listPp)
    {

        var filteredAv = DbFilter.Avani.ToList();
        var filteredPp = DbGlobal.Pp.ToList();

        filteredAv.RemoveAll(x => listAv.Exists(y => y.Pad == x.Pad));

        List<Avani> tempListAvani = new List<Avani>();
       
        //calcualte every distance foreach points in lists
        for (int i = 0; i <= listAv.Count - 1; i++)
        {
            for (int k = 0; k <= filteredAv.Count - 1; k++)
            {
                var result = Math.Sqrt(Math.Pow(listAv[i].Lx - filteredAv[k].Lx, 2) + Math.Pow(listAv[i].Ly - filteredAv[k].Ly, 2));

                //if result is lower than input text save items in list
                if (result <= inputFang2)
                {
                        if(filteredAv[k].Part.Equals("PS2")|| filteredAv[k].Part.Equals("PS3") || filteredAv[k].Part.Equals("PS4"))
                            tempListAvani.Add(filteredAv[k]);
                }
            }
        }
        //elimante duplicates from filteredAv and filteredpp
        filteredPp.RemoveAll(x => listAv.Exists(y => x.PAD == y.Pad));
        //eliminate every duplicate from result list
        IEnumerable<Avani> distinctListAvani = tempListAvani.Distinct();

        //Add result points to final Av list
        foreach (var item in distinctListAvani)
        {
            listAv.Add(item);


        }

        //compare finalAv to filteredPp and add missing points to dataGrid
        listPp.AddRange(filteredPp.Where(x => listAv.Exists(y => y.Pad == x.PAD)));

        }
    /// <summary>
    /// Filter if both parameter are used
    /// </summary>
    /// <param name="listAv">Avani list</param>
    /// <param name="inputFang1">PS0/PS1 Textbox input</param>
    /// <param name="inputFang2">PS"-PS4 Textbox input</param>
    /// <param name="listPp">Pp list</param>
    public static void FangradiusPS0PS4(List<Avani> listAv,double inputFang1, double inputFang2, List<Pp> listPp)
    {

        var filteredAv = DbFilter.Avani.ToList();
        var filteredPp = DbGlobal.Pp.ToList();

        filteredAv.RemoveAll(x => listAv.Exists(y => y.Pad == x.Pad));

        List<Avani> tempListAvani = new List<Avani>();
        

        //calcualte every distance foreach points in lists
        for (int i = 0; i <= listAv.Count - 1; i++)
        {
            for (int k = 0; k <= filteredAv.Count - 1; k++)
            {
                var result = Math.Sqrt(Math.Pow(listAv[i].Lx - filteredAv[k].Lx, 2) + Math.Pow(listAv[i].Ly - filteredAv[k].Ly, 2));

                    //search for PS0/PS1 points, which are in distance
                    if (result <= inputFang1 && (filteredAv[k].Part.Equals("PS0") || filteredAv[k].Part.Equals("PS1")))
                    {
                        tempListAvani.Add(filteredAv[k]);
                    }
                    ////search for PS2-PS4 points, which are in distance 
                    if (result <= inputFang2 && (filteredAv[k].Part.Equals("PS2") || filteredAv[k].Part.Equals("PS3") || filteredAv[k].Part.Equals("PS4")))
                {
                        tempListAvani.Add(filteredAv[k]);
                }
            }
        }
        //elimante duplicates from filteredAv and filteredpp
        filteredPp.RemoveAll(x => listAv.Exists(y => x.PAD == y.Pad));
        //eliminate every duplicate from result list
        IEnumerable<Avani> distinctListAvani = tempListAvani.Distinct();

        //Add result points to final Av list
        foreach (var item in distinctListAvani)
        {
            listAv.Add(item);


        }

        //compare finalAv to filteredPp and add missing points to dataGrid
        listPp.AddRange(filteredPp.Where(x => listAv.Exists(y => y.Pad == x.PAD)));

    }
    }
}