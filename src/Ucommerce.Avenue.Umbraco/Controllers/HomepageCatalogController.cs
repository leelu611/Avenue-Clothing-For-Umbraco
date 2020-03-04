﻿using System;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class HomepageCatalogController : SurfaceController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public CatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<CatalogLibrary>();

        // GET: HomepageCatalog
        public ActionResult Index()
        {
            var products = SiteContext.Current.CatalogContext.CurrentCatalog.Categories.SelectMany(c =>
                c.Products.Where(p => p.ProductProperties.Any(pp =>
                    pp.ProductDefinitionField.Name == "ShowOnHomepage" && !String.IsNullOrEmpty(pp.Value) &&
                    Convert.ToBoolean(pp.Value))));
            ProductsViewModel productsViewModel = new ProductsViewModel();

            foreach (var p in products)
            {
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Name = p.Name,
                    PriceCalculation = CatalogLibrary.CalculatePrices(p),
                    Url = UrlService.GetUrl(p),
                    Sku = p.Sku,
                    IsVariant = p.IsVariant,
                    VariantSku = p.VariantSku,
                    ThumbnailImageUrl = ObjectFactory.Instance.Resolve<IImageService>()
                        .GetImage(p.ThumbnailImageMediaId).Url
                });
            }

            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}